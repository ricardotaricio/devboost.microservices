using AutoMapper;
using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Application.Extensions;
using DevBoost.DroneDelivery.Core.Domain.Enumerators;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
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
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IDroneItinerarioRepository _droneItinerarioRepository;
        private readonly IDroneRepository _droneRepository;
        private const double _latitudeLoja = -23.5880684;
        private const double _longitudeLoja = -46.6564195;
        private Localizacao _localizacaoLoja;
        private IMediatrHandler _mediatr;
        private IMapper _mapper;
        public PedidoService(IMapper mapper, IMediatrHandler mediatr, IPedidoRepository repositoryPedido,
            IDroneItinerarioRepository droneItinerarioRepository,
            IDroneRepository droneRepository)
        {
            _mediatr = mediatr;
            _pedidoRepository = repositoryPedido;
            _droneItinerarioRepository = droneItinerarioRepository;
            _droneRepository = droneRepository;
            _localizacaoLoja = new Localizacao(-23.5880684, -46.6564195);
            _mapper = mapper;
        }

        public async Task<IEnumerable<Pedido>> GetAll()
        {
            return await _pedidoRepository.ObterTodos();
        }

        public async Task<Pedido> GetById(Guid id)
        {
            var pedido = await _pedidoRepository.ObterPorId(id);

            if (pedido.DroneId != Guid.Empty)
                pedido.Drone = await _droneRepository.ObterPorId(pedido.DroneId);

            return pedido;
        }

        public async Task<bool> Insert(Pedido pedido)
        {
            var dronesSitema = await _droneRepository.ObterTodos();

            if (!dronesSitema.Any(d => d.Capacidade >= pedido.Peso))
                return await Task.Run(() => false);

            await _pedidoRepository.Adicionar(pedido);
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Update(Pedido pedido)
        {
            await _pedidoRepository.Atualizar(pedido);
            return await _pedidoRepository.UnitOfWork.Commit();

        }

        public async Task<IEnumerable<Pedido>> GetPedidosEmTransito()
        {
            return await _pedidoRepository.ObterPedidosEmTransito();
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
                        await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneItinerario.DroneId, EnumStatusDrone.Disponivel));

                }
                else if (droneItinerario.StatusDrone == EnumStatusDrone.EmTransito)
                {
                    var pedidos = _pedidoRepository.ObterPedidosEmTransito().Result.Where(p => p.Status == EnumStatusPedido.EmTransito && p.Drone.Id == droneItinerario.DroneId).ToList();


                    int tempoEntrega = CalcularTempoTotalEntregaEmMinutos(pedidos, droneItinerario.Drone);

                    if (droneItinerario.DataHora.AddMinutes(tempoEntrega) <= DateTime.Now)
                    {

                        int limiteAutonomiaParaRecarga = Convert.ToInt32(Math.Ceiling(droneItinerario.Drone.Autonomia * 0.2));

                        if (droneItinerario.Drone.AutonomiaRestante <= limiteAutonomiaParaRecarga)
                            await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneItinerario.DroneId, EnumStatusDrone.Carregando));
                        else
                            await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneItinerario.DroneId, EnumStatusDrone.Disponivel));

                        foreach (var pedido in pedidos)
                        {
                            pedido.InformarStatus(EnumStatusPedido.Entregue);
                            await _pedidoRepository.Atualizar(pedido);
                        }


                    }
                }
            }

        }

        private async Task DistribuirPedidos()
        {
            var pedidos = new List<Pedido>();

            // pedidos mais antigos vao primeiro
            pedidos = _pedidoRepository.ObterPedidosEmAberto().Result.ToList();


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
                            latitudeOrigem = pedidosEntregar.Last().Cliente.Latitude;
                            longitudeOrigem = pedidosEntregar.Last().Cliente.Longitude;
                        }
                        else
                        {
                            latitudeOrigem = _latitudeLoja;
                            longitudeOrigem = _longitudeLoja;
                        }

                        distanciaTrajeto = CalcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, pedido.Cliente.Latitude, pedido.Cliente.Longitude);
                        distanciaRetorno = CalcularDistanciaEmKilometros(pedido.Cliente.Latitude, pedido.Cliente.Longitude, _latitudeLoja, _longitudeLoja);

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
                    await _mediatr.EnviarComando(new AtualizarAutonomiaDroneCommand(drone.Id, autonomiaDisponivel));

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
                    var drone = item.Key;


                    await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, drone.Id, EnumStatusDrone.EmTransito));

                    foreach (var pedido in item.Value)
                    {
                        pedido.Drone = drone;
                        pedido.InformarStatus(EnumStatusPedido.EmTransito);

                        await _pedidoRepository.Atualizar(pedido);
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
                await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneId, EnumStatusDrone.Disponivel));
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

                distanciaTotal += CalcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, pedido.Cliente.Latitude,pedido.Cliente.Longitude);

                // origem do proximo trajeto
                latitudeOrigem = pedido.Cliente.Latitude;
                longitudeOrigem = pedido.Cliente.Longitude;
            }

            // retorno para loja
            distanciaTotal += CalcularDistanciaEmKilometros(latitudeOrigem, longitudeOrigem, _latitudeLoja, _longitudeLoja);

            int tempo = CalcularTempoTrajetoEmMinutos(distanciaTotal, drone.Velocidade);

            return tempo;
        }

        public string IsPedidoValido(Pedido pedido)
        {
            var drone = _droneRepository.ObterTodos().Result.FirstOrDefault(d => d.Capacidade >= pedido.Peso);

            if (drone == null)
                return "Pedido acima do peso máximo aceito.";

            double distancia = _localizacaoLoja.CalcularDistanciaEmKilometros(new Localizacao(pedido.Cliente.Latitude, pedido.Cliente.Longitude));
            distancia *= 2;

            int tempoTrajetoCompleto = _localizacaoLoja.CalcularTempoTrajetoEmMinutos(distancia, drone.Velocidade);

            if (tempoTrajetoCompleto > drone.Autonomia)
                return "Fora da área de entrega.";

            return String.Empty;
        }

        
    }
}
