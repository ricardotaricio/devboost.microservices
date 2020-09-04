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
        private const double _latitudeLoja = -23.5880684;
        private const double _longitudeLoja = -46.6564195;
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

        public async Task<IList<Pedido>> GetAll()
        {
            //await DespacharPedidos();
            return await _repositoryPedido.GetAll();
        }

        public async Task<Pedido> GetById(Guid id)
        {
            var pedido = await _repositoryPedido.GetById(id);

            if (pedido.DroneId > 0)
                pedido.Drone = await _droneRepository.GetById(pedido.DroneId);

            return pedido;
        }

        public async Task<bool> Insert(Pedido pedido)
        {
            var dronesSitema = await _droneRepository.GetAll();

            if (!dronesSitema.Any(d => d.Capacidade >= pedido.Peso))
                return await Task.Run(() => false);    
            
            //Deve colocar a regra de criação de pedido

            return await _repositoryPedido.Insert(pedido);
        }

        public async Task<Pedido> Update(Pedido pedido)
        {
            return await _repositoryPedido.Update(pedido);
        }

        public async Task<IList<Pedido>> GetPedidosEmTransito()
        {
            return await _repositoryPedido.GetPedidosEmTransito();
        }

        public async Task DespacharPedidos()
        {
            await atualizarStatusDrones();
            await distribuirPedidos();
        }

        public async Task atualizarStatusDrones()
        {
            await criarDroneItinerario();

            var droneItinerarios = _droneItinerarioRepository.GetAll().Result.Where(d => d.StatusDrone != EnumStatusDrone.Disponivel).ToList();

            foreach (var droneItinerario in droneItinerarios)
            {
                //droneItinerario.Drone = _unitOfWork.Drones.GetById(droneItinerario.DroneId);

                if (droneItinerario.StatusDrone == EnumStatusDrone.Carregando)
                {
                    if (DateTime.Now.Subtract(droneItinerario.DataHora).TotalMinutes >= droneItinerario.Drone.Carga)
                    {
                        droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;
                        droneItinerario.Drone.AutonomiaRestante = droneItinerario.Drone.Autonomia;
                        droneItinerario.DataHora = DateTime.Now;

                        await _droneItinerarioRepository.Update(droneItinerario);
                    }
                }
                else if (droneItinerario.StatusDrone == EnumStatusDrone.EmTransito)
                {
                    var pedidos = _repositoryPedido.GetPedidosEmTransito().Result.Where(p => p.Status == EnumStatusPedido.EmTransito && p.Drone.Id == droneItinerario.DroneId).ToList();

                    // int tempoEntrega = pedido.PrevisaoEntrega.Subtract(pedido.DataHora).Minutes;
                    int tempoEntrega = CalcularTempoTotalEntregaEmMinutos(pedidos, droneItinerario.Drone);

                    if (droneItinerario.DataHora.AddMinutes(tempoEntrega) <= DateTime.Now)
                    {
                        // se autonomia ficar abaixo de 20%, recarrega
                        int limiteAutonomiaParaRecarga = Convert.ToInt32(Math.Ceiling(droneItinerario.Drone.Autonomia * 0.2));

                        if (droneItinerario.Drone.AutonomiaRestante <= limiteAutonomiaParaRecarga)
                            droneItinerario.StatusDrone = EnumStatusDrone.Carregando;
                        else
                            droneItinerario.StatusDrone = EnumStatusDrone.Disponivel;

                        droneItinerario.DataHora = DateTime.Now;

                        foreach (var pedido in pedidos)
                        {
                            pedido.InformarStatus(EnumStatusPedido.Entregue);
                            await _repositoryPedido.Update(pedido);
                        }

                        await _droneItinerarioRepository.Update(droneItinerario);
                    }
                }
            }

        }

        private async Task distribuirPedidos()
        {
            IList<Pedido> pedidos = new List<Pedido>();

            try
            {
                // pedidos mais antigos vao primeiro
                pedidos = _repositoryPedido.GetPedidosEmAberto().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (!pedidos.Any())
                return;

            var droneItinerarios = _droneItinerarioRepository.GetAll().Result
                .Where(d => d.StatusDrone == EnumStatusDrone.Disponivel)
                .ToList();

            foreach (var droneItinerario in droneItinerarios)
            {
                droneItinerario.Drone = await _droneRepository.GetById(droneItinerario.DroneId);
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
            double latitudeOrigem = 0;
            double longitudeOrigem = 0;
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
                            latitudeOrigem = (double)pedidosEntregar.Last().Cliente.Latitude;
                            longitudeOrigem = (double)pedidosEntregar.Last().Cliente.Longitude;
                        }
                        else
                        {
                            latitudeOrigem = _latitudeLoja;
                            longitudeOrigem = _longitudeLoja;
                        }

                        distanciaTrajeto = calcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, (double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude);
                        distanciaRetorno = calcularDistanciaEmKilometros((double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude, _latitudeLoja, _longitudeLoja);

                        distanciaTotal = distanciaPercorrida + distanciaTrajeto + distanciaRetorno;

                        tempoTrajetoCompleto = calcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

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

                    DroneItinerario droneItinerario = droneItinerarios.Where(d => d.DroneId == drone.Id).FirstOrDefault();
                    droneItinerario.DataHora = DateTime.Now;
                    droneItinerario.Drone = drone;
                    droneItinerario.DroneId = drone.Id;
                    droneItinerario.StatusDrone = EnumStatusDrone.EmTransito;

                    await _droneRepository.Update(drone);
                    await _droneItinerarioRepository.Update(droneItinerario);
                                                           
                    foreach (var pedido in item.Value)
                    {
                        pedido.Drone = drone;
                        pedido.InformarStatus(EnumStatusPedido.EmTransito);

                        await _repositoryPedido.Update(pedido);
                    }
                }
            }

        }

        private async Task criarDroneItinerario()
        {
            var dronesId = _droneRepository.GetAll().Result.Select(d => d.Id).ToList();

            var droneItininerarios = _droneItinerarioRepository.GetAll().Result.Select(i => i.DroneId).ToList();

            var dronesSemItinerario = dronesId.Except(droneItininerarios).ToList();

            foreach (var droneId in dronesSemItinerario)
            {
                DroneItinerario droneItinerario = new DroneItinerario();
                //droneItinerario.Drone = _unitOfWork.Drones.GetById(droneId);
                droneItinerario.InformarDataHora(DateTime.Now);
                droneItinerario.InformarStatusDrone(EnumStatusDrone.Disponivel);
                droneItinerario.InformarDrone(_droneRepository.GetById(droneId).Result);
                await _droneItinerarioRepository.Insert(droneItinerario);                
            }
        }
                
        private double calcularDistanciaEmKilometros(double latitudeOrigem, double longitudeOrigem, double latitudeDestino, double longitudeDestino)
        {
            var origemCoord = new GeoCoordinate(latitudeOrigem, longitudeOrigem);
            var destinoCoord = new GeoCoordinate(latitudeDestino, longitudeDestino);

            var distance = origemCoord.GetDistanceTo(destinoCoord);

            distance = distance / 1000;

            return distance;
        }

        private int calcularTempoTrajetoEmMinutos(double distanciaEmKilometros, int velocidadeEmKilometrosPorHora)
        {
            double tempo = distanciaEmKilometros / velocidadeEmKilometrosPorHora;

            tempo = tempo * 60;

            return Convert.ToInt32(Math.Ceiling(tempo));
        }

        public int CalcularTempoTotalEntregaEmMinutos(IList<Pedido> pedidos, Drone drone)
        {
            if (!pedidos.Any())
                return 0;

            double latitudeOrigem = 0;
            double longitudeOrigem = 0;
            double distanciaTotal = 0;

            foreach (var pedido in pedidos)
            {
                // primeiro trajeto sai da loja
                if (distanciaTotal == 0)
                {
                    latitudeOrigem = _latitudeLoja;
                    longitudeOrigem = _longitudeLoja;
                }

                distanciaTotal += calcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, (double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude);

                // origem do proximo trajeto
                latitudeOrigem = (double)pedido.Cliente.Latitude;
                longitudeOrigem = (double)pedido.Cliente.Longitude;
            }

            // retorno para loja
            distanciaTotal += calcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, _latitudeLoja, _longitudeLoja);

            int tempo = calcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

            return tempo;
        }

        public string IsPedidoValido(Pedido pedido)
        {
            var drone = _droneRepository.GetAll().Result.Where(d => d.Capacidade >= pedido.Peso).FirstOrDefault();

            if (drone == null)
                return "Pedido acima do peso máximo aceito.";

            double distancia = _localizacaoLoja.CalcularDistanciaEmKilometros(new Localizacao(pedido.Cliente.Latitude, pedido.Cliente.Longitude));
            // double distancia = calcularDistanciaEmKilometros(_latitudeLoja, _longitudeLoja, (double)pedido.Cliente.Latitude, (double)pedido.Cliente.Longitude);
            distancia *= 2;
            
            int tempoTrajetoCompleto = _localizacaoLoja.CalcularTempoTrajetoEmMinutos(distancia, drone.Velocidade);

            if (tempoTrajetoCompleto > drone.Autonomia)
                return "Fora da área de entrega.";

            return String.Empty;
        }
    }
}
