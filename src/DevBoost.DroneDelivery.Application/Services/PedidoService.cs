using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.Extensions;
using DevBoost.DroneDelivery.Domain.Entities;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _repositoryPedido;
        private readonly IDroneItinerarioRepository _droneItinerarioRepository;
        private readonly IDroneRepository _droneRepository;
        private const decimal _latitudeLoja = (decimal)-23.5880684;
        private const decimal _longitudeLoja = (decimal)-46.6564195;
        private Localizacao _localizacaoLoja;

        public PedidoService(IPedidoRepository repositoryPedido,
            IDroneItinerarioRepository droneItinerarioRepository,
            IDroneRepository droneRepository)
        {
            _repositoryPedido = repositoryPedido;
            _droneItinerarioRepository = droneItinerarioRepository;
            _droneRepository = droneRepository;
            _localizacaoLoja = new Localizacao(-23.5880684, -46.6564195);
        }

        public async Task<IEnumerable<Pedido>> GetAll()
        {
            return await _repositoryPedido.ObterTodos();
        }

        public async Task<Pedido> GetById(Guid id)
        {
            var pedido = await _repositoryPedido.ObterPorId(id);

            if (pedido.DroneId > 0)
                pedido.Drone = await _droneRepository.ObterPorId(pedido.DroneId);

            return pedido;
        }

        public async Task<bool> Insert(Pedido pedido)
        {
            var dronesSitema = await _droneRepository.ObterTodos();

            if (!dronesSitema.Any(d => d.Capacidade >= pedido.Peso))
                return await Task.Run(() => false);

            await _repositoryPedido.Adicionar(pedido);
            return await _repositoryPedido.UnitOfWork.Commit();
        }

        public async Task<bool> Update(Pedido pedido)
        {
            await _repositoryPedido.Atualizar(pedido);
            return await _repositoryPedido.UnitOfWork.Commit();

        }

        public async Task<IEnumerable<Pedido>> GetPedidosEmTransito()
        {
            return await _repositoryPedido.ObterPedidosEmTransito();
        }

        public async Task DespacharPedidos()
        {
            await AtualizarStatusDrones();
            await DistribuirPedidos();
        }

        public async Task AtualizarStatusDrones()
        {
            await CriarDroneItinerario();

            var droneItinerarios = _droneItinerarioRepository.ObterTodos().Result.Where(d => d.StatusDrone != EnumStatusDrone.Disponivel).ToList();

            foreach (var droneItinerario in droneItinerarios)
            {


                if (droneItinerario.StatusDrone == EnumStatusDrone.Carregando)
                {
                    if (DateTime.Now.Subtract(droneItinerario.DataHora).TotalMinutes >= droneItinerario.Drone.Carga)
                    {
                        droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;
                        droneItinerario.Drone.AutonomiaRestante = droneItinerario.Drone.Autonomia;
                        droneItinerario.DataHora = DateTime.Now;

                        await _droneItinerarioRepository.Atualizar(droneItinerario);
                    }
                }
                else if (droneItinerario.StatusDrone == EnumStatusDrone.EmTransito)
                {
                    var pedidos = _repositoryPedido.ObterPedidosEmTransito().Result.Where(p => p.Status == EnumStatusPedido.EmTransito && p.Drone.Id == droneItinerario.DroneId).ToList();


                    int tempoEntrega = CalcularTempoTotalEntregaEmMinutos(pedidos, droneItinerario.Drone);

                    if (droneItinerario.DataHora.AddMinutes(tempoEntrega) <= DateTime.Now)
                    {

                        int limiteAutonomiaParaRecarga = Convert.ToInt32(Math.Ceiling(droneItinerario.Drone.Autonomia * 0.2));

                        if (droneItinerario.Drone.AutonomiaRestante <= limiteAutonomiaParaRecarga)
                            droneItinerario.StatusDrone = EnumStatusDrone.Carregando;
                        else
                            droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;

                        droneItinerario.DataHora = DateTime.Now;

                        foreach (var pedido in pedidos)
                        {
                            pedido.InformarStatus(EnumStatusPedido.Entregue);
                            await _repositoryPedido.Atualizar(pedido);
                        }

                        await _droneItinerarioRepository.Atualizar(droneItinerario);
                    }
                }
            }

        }

        private async Task DistribuirPedidos()
        {
            var pedidos = new List<Pedido>();


            // pedidos mais antigos vao primeiro
            pedidos = _repositoryPedido.ObterPedidosEmAberto().Result.ToList();


            if (!pedidos.Any())
                return;

            var droneItinerarios = _droneItinerarioRepository.ObterTodos().Result
                .Where(d => d.StatusDrone == EnumStatusDrone.Disponivel)
                .ToList();

            foreach (var droneItinerario in droneItinerarios)
            {
                droneItinerario.Drone = await _droneRepository.ObterPorId(droneItinerario.DroneId);
            }

            var dronesDisponiveis = droneItinerarios.Select(d => d.Drone).OrderBy(d => d.Capacidade).ToList();

            if (!dronesDisponiveis.Any())
                return;

            IDictionary<Drone, IList<Pedido>> pedidosPorDrone = new Dictionary<Drone, IList<Pedido>>();
            IList<Pedido> pedidosEntregar = new List<Pedido>();
            IList<Pedido> pedidosDistribuidos = new List<Pedido>();
            double distanciaTrajeto = 0;
            double distanciaRetorno = 0;
            double distanciaPercorrida = 0;
            double distanciaTotal = 0;
            decimal latitudeOrigem = 0;
            decimal longitudeOrigem = 0;
            int tempoTrajetoCompleto = 0;

            foreach (var drone in dronesDisponiveis)
            {
                pedidosEntregar.Clear();

                distanciaTrajeto = 0;
                distanciaRetorno = 0;
                distanciaPercorrida = 0;
                distanciaTotal = 0;
                latitudeOrigem = 0;
                longitudeOrigem = 0;
                tempoTrajetoCompleto = 0;

                int capacidadeDisponivel = drone.Capacidade;
                int autonomiaDisponivel = drone.AutonomiaRestante;

                foreach (var pedido in pedidos)
                {
                    if (pedidosDistribuidos.Contains(pedido))
                        continue;

                    if (pedido.Peso <= capacidadeDisponivel)
                    {
                        if (pedidosEntregar.Any())
                        {
                            latitudeOrigem = pedidosEntregar.Last().Cliente.Latitude;
                            longitudeOrigem = pedidosEntregar.Last().Cliente.Longitude;
                        }
                        else
                        {
                            latitudeOrigem = _latitudeLoja;
                            longitudeOrigem = _longitudeLoja;
                        }

                        distanciaTrajeto = CalcularDistanciaEmKilometros((double)latitudeOrigem, (double)longitudeOrigem, (double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude);
                        distanciaRetorno = CalcularDistanciaEmKilometros((double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude, (double)_latitudeLoja, (double)_longitudeLoja);

                        distanciaTotal = distanciaPercorrida + distanciaTrajeto + distanciaRetorno;

                        tempoTrajetoCompleto = CalcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

                        if (tempoTrajetoCompleto <= drone.AutonomiaRestante)
                        {
                            pedidosEntregar.Add(pedido);

                            distanciaPercorrida += distanciaTrajeto;

                            capacidadeDisponivel -= pedido.Peso;
                            autonomiaDisponivel = drone.AutonomiaRestante - tempoTrajetoCompleto;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // se não cabe mais, nao precisa verificar os demais pedidos
                    if (capacidadeDisponivel <= 0 || tempoTrajetoCompleto >= drone.AutonomiaRestante)
                    {
                        break;
                    }
                }

                if (pedidosEntregar.Any())
                {
                    drone.AutonomiaRestante = autonomiaDisponivel;

                    if (!pedidosPorDrone.ContainsKey(drone))
                        pedidosPorDrone.Add(drone, new List<Pedido>());

                    foreach (var pedido in pedidosEntregar)
                    {
                        pedidosPorDrone[drone].Add(pedido);
                    }

                    pedidosDistribuidos = pedidosDistribuidos.Concat(pedidosEntregar).ToList();
                }
            }

            if (pedidosPorDrone.Count > 0)
            {
                foreach (var item in pedidosPorDrone)
                {
                    Drone drone = item.Key;

                    DroneItinerario droneItinerario = droneItinerarios.FirstOrDefault(d => d.DroneId == drone.Id);
                    droneItinerario.DataHora = DateTime.Now;
                    droneItinerario.Drone = drone;
                    droneItinerario.DroneId = drone.Id;
                    droneItinerario.StatusDrone = EnumStatusDrone.EmTransito;

                    await _droneRepository.Atualizar(drone);
                    await _droneItinerarioRepository.Atualizar(droneItinerario);

                    foreach (var pedido in item.Value)
                    {
                        pedido.Drone = drone;
                        pedido.InformarStatus(EnumStatusPedido.EmTransito);

                        await _repositoryPedido.Atualizar(pedido);
                    }
                }
            }

        }

        private async Task CriarDroneItinerario()
        {
            var dronesId = _droneRepository.ObterTodos().Result.Select(d => d.Id).ToList();

            var droneItininerarios = _droneItinerarioRepository.ObterTodos().Result.Select(i => i.DroneId).ToList();

            var dronesSemItinerario = dronesId.Except(droneItininerarios).ToList();

            foreach (var droneId in dronesSemItinerario)
            {
                DroneItinerario droneItinerario = new DroneItinerario();
                droneItinerario.InformarDataHora(DateTime.Now);
                droneItinerario.InformarStatusDrone(EnumStatusDrone.Disponivel);
                droneItinerario.InformarDrone(_droneRepository.ObterPorId(droneId).Result);
                await _droneItinerarioRepository.Adicionar(droneItinerario);
            }
        }

        private double CalcularDistanciaEmKilometros(double latitudeOrigem, double longitudeOrigem, double latitudeDestino, double longitudeDestino)
        {
            var origemCoord = new GeoCoordinate(latitudeOrigem, longitudeOrigem);
            var destinoCoord = new GeoCoordinate(latitudeDestino, longitudeDestino);

            var distance = origemCoord.GetDistanceTo(destinoCoord);

            distance = distance / 1000;

            return distance;
        }

        private int CalcularTempoTrajetoEmMinutos(double distanciaEmKilometros, int velocidadeEmKilometrosPorHora)
        {
            double tempo = distanciaEmKilometros / velocidadeEmKilometrosPorHora;

            tempo = tempo * 60;

            return Convert.ToInt32(Math.Ceiling(tempo));
        }

        public int CalcularTempoTotalEntregaEmMinutos(IList<Pedido> pedidos, Drone drone)
        {
            if (!pedidos.Any())
                return 0;

            decimal latitudeOrigem = 0;
            decimal longitudeOrigem = 0;
            double distanciaTotal = 0;

            foreach (var pedido in pedidos)
            {
                // primeiro trajeto sai da loja
                if (distanciaTotal == 0)
                {
                    latitudeOrigem = _latitudeLoja;
                    longitudeOrigem = _longitudeLoja;
                }

                distanciaTotal += CalcularDistanciaEmKilometros((double)latitudeOrigem, (double)longitudeOrigem, (double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude);

                // origem do proximo trajeto
                latitudeOrigem = pedido.Cliente.Latitude;
                longitudeOrigem = pedido.Cliente.Longitude;
            }

            // retorno para loja
            distanciaTotal += CalcularDistanciaEmKilometros((double)latitudeOrigem, (double)longitudeOrigem, (double)_latitudeLoja, (double)_longitudeLoja);

            int tempo = CalcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

            return tempo;
        }

        public string IsPedidoValido(Pedido pedido)
        {
            var drone = _droneRepository.ObterTodos().Result.FirstOrDefault(d => d.Capacidade >= pedido.Peso);

            if (drone == null)
                return "Pedido acima do peso máximo aceito.";

            double distancia = _localizacaoLoja.CalcularDistanciaEmKilometros(new Localizacao((double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude));
            distancia *= 2;

            int tempoTrajetoCompleto = _localizacaoLoja.CalcularTempoTrajetoEmMinutos(distancia, drone.Velocidade);

            if (tempoTrajetoCompleto > drone.Autonomia)
                return "Fora da área de entrega.";

            return String.Empty;
        }
    }
}
