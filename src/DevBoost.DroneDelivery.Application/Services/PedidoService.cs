using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Application.Extensions;
using DevBoost.DroneDelivery.Application.Queries;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Domain.Interfaces.Services;
using DevBoost.DroneDelivery.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoQueries _pedidoQueries;
        private readonly IDroneItinerarioQueries _droneItinerarioQueries;
        private Localizacao _localizacaoLoja;
        private readonly IMediatrHandler _mediatr;
        
        public PedidoService(IDroneItinerarioQueries droneItinerarioQueries, IMediatrHandler mediatr, IPedidoQueries pedidoQueries)
        {
            _mediatr = mediatr;
            _pedidoQueries = pedidoQueries;
            _localizacaoLoja = new Localizacao(-23.5880684, -46.6564195);
            _droneItinerarioQueries = droneItinerarioQueries;
        }


        public async Task DespacharPedidos()
        {
            await AtualizarStatusDrones();
            await DistribuirPedidos();
        }

        public async Task AtualizarStatusDrones()
        {

            var droneItinerarios = _droneItinerarioQueries.ObterDronesIndisponiveis().Result.ToList();

            foreach (var droneItinerario in droneItinerarios)
            {


                if (droneItinerario.StatusDrone == EnumStatusDrone.Carregando)
                {
                    if (DateTime.Now.Subtract(droneItinerario.DataHora).TotalMinutes >= droneItinerario.Drone.Carga)
                        await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneItinerario.Drone.Id, EnumStatusDrone.Disponivel));

                }
                else if (droneItinerario.StatusDrone == EnumStatusDrone.EmTransito)
                {
                    var pedidos = await _pedidoQueries.ObterPedidosEmTransitoPorDrone(droneItinerario.Drone.Id);


                    int tempoEntrega = CalcularTempoTotalEntregaEmMinutos(pedidos, droneItinerario.Drone);

                    if (droneItinerario.DataHora.AddMinutes(tempoEntrega) <= DateTime.Now)
                    {

                        int limiteAutonomiaParaRecarga = Convert.ToInt32(Math.Ceiling(droneItinerario.Drone.Autonomia * 0.2));

                        if (droneItinerario.Drone.AutonomiaRestante <= limiteAutonomiaParaRecarga)
                            await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneItinerario.Drone.Id, EnumStatusDrone.Carregando));
                        else
                            await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, droneItinerario.Drone.Id, EnumStatusDrone.Disponivel));

                        foreach (var pedido in pedidos)
                            await _mediatr.EnviarComando(new AtualizarSituacaoPedidoCommand(pedido.Id, EnumStatusPedido.Entregue));

                    }
                }
            }

        }

        private async Task DistribuirPedidos()
        {

            var pedidos = _pedidoQueries.ObterPedidosEmAberto().Result.ToList();
            if (!pedidos.Any())
                return;

            var dronesDisponiveis = _droneItinerarioQueries.ObterDronesDisponiveis().Result.ToList();
            if (!dronesDisponiveis.Any())
                return;

            var pedidosEntregar = new List<PedidoViewModel>();
            var pedidosDistribuidos = new List<PedidoViewModel>();
            foreach (var drone in dronesDisponiveis)
            {
                pedidosEntregar.Clear();

                double distanciaTrajeto = 0;
                double distanciaRetorno = 0;
                double distanciaPercorrida = 0;
                double distanciaTotal = 0;
                int tempoTrajetoCompleto = 0;
                var capacidadeDisponivel = drone.Capacidade;
                var autonomiaDisponivel = drone.AutonomiaRestante;

                foreach (var pedido in pedidos)
                {
                    if (pedidosDistribuidos.Contains(pedido))
                        continue;

                    Localizacao localizacaoOrigem;

                    if (pedido.Peso > capacidadeDisponivel) continue;


                    if (pedidosEntregar.Any())
                        localizacaoOrigem = new Localizacao(pedidosEntregar.Last().Cliente.Latitude, pedidosEntregar.Last().Cliente.Longitude);
                    else
                        localizacaoOrigem = _localizacaoLoja;


                    var localizacaoCliente = new Localizacao(pedido.Cliente.Latitude, pedido.Cliente.Longitude);

                    distanciaTrajeto = localizacaoOrigem.CalcularDistanciaEmKilometros(localizacaoCliente);
                    distanciaRetorno = localizacaoCliente.CalcularDistanciaEmKilometros(localizacaoOrigem);

                    distanciaTotal = distanciaPercorrida + distanciaTrajeto + distanciaRetorno;

                    tempoTrajetoCompleto = distanciaTotal.CalcularTempoTrajetoEmMinutos(drone.Velocidade);

                    if (tempoTrajetoCompleto <= drone.AutonomiaRestante)
                    {
                        pedidosEntregar.Add(pedido);

                        distanciaPercorrida += distanciaTrajeto;

                        capacidadeDisponivel -= pedido.Peso;
                        autonomiaDisponivel = drone.AutonomiaRestante - tempoTrajetoCompleto;
                    }


                    // se não cabe mais, nao precisa verificar os demais pedidos
                    if (capacidadeDisponivel <= 0 || tempoTrajetoCompleto >= drone.AutonomiaRestante) break;

                }

                if (!pedidosEntregar.Any()) continue;


                await _mediatr.EnviarComando(new AtualizarAutonomiaDroneCommand(drone.Id, autonomiaDisponivel));
                await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, drone.Id, EnumStatusDrone.EmTransito));

                foreach (var pedido in pedidosEntregar)
                {
                    await _mediatr.EnviarComando(new DespacharPedidoCommand(pedido.Id, drone.Id, EnumStatusPedido.EmTransito));
                    pedidos.Remove(pedido);
                }


            }



        }


        public int CalcularTempoTotalEntregaEmMinutos(IEnumerable<PedidoViewModel> pedidos, DroneViewModel drone)
        {
            if (!pedidos.Any())
                return 0;

            var localizacaoOrigem = _localizacaoLoja;
            Localizacao localizacaoCliente;

            double distanciaTotal = 0;

            foreach (var pedido in pedidos)
            {
                localizacaoCliente = new Localizacao(pedido.Cliente.Latitude, pedido.Cliente.Longitude);
                distanciaTotal += localizacaoOrigem.CalcularDistanciaEmKilometros(localizacaoCliente);
                localizacaoOrigem = localizacaoCliente;
            }

            distanciaTotal += localizacaoOrigem.CalcularDistanciaEmKilometros(_localizacaoLoja);

            return distanciaTotal.CalcularTempoTrajetoEmMinutos(drone.Velocidade);

        }

    }
}
