using DevBoost.DroneDelivery.Domain.Interfaces;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Application.Events;
using DevBoost.DroneDelivery.Application.ViewModels;
using DevBoost.DroneDelivery.Application.Queries;
using System.Linq;
using System.Collections.Generic;
using DevBoost.DroneDelivery.Domain.ValueObjects;
using DevBoost.DroneDelivery.Application.Extensions;
using System;
using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Core.Domain.Enumerators;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarPedidoCommand, bool>, IRequestHandler<AtualizarSituacaoPedidoCommand, bool>, IRequestHandler<DespacharPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IDroneRepository _droneRepository;
        private readonly IUsuarioAutenticado _usuarioAutenticado;
        private readonly IUserRepository _userRepository;
        private readonly IMediatrHandler _mediatr;
        public readonly IPedidoQueries _pedidoQueries;
        public readonly IDroneItinerarioQueries  _droneItinerarioQueries;

        public PedidoCommandHandler(IDroneItinerarioQueries droneItinerarioQueries, IPedidoQueries pedidoQueries, IDroneRepository droneRepository, IMediatrHandler mediatr, IPedidoRepository repositoryPedido, IUsuarioAutenticado usuarioAutenticado, IUserRepository userRepository)
        {
            _pedidoRepository = repositoryPedido;
            _usuarioAutenticado = usuarioAutenticado;
            _userRepository = userRepository;
            _mediatr = mediatr;
            _droneRepository = droneRepository;
            _pedidoQueries = pedidoQueries;
            _droneItinerarioQueries = droneItinerarioQueries;
        }

        public async Task<bool> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var user = await _userRepository.ObterPorNome(_usuarioAutenticado.GetCurrentUserName());
            if (user.Cliente == null)
                return false;

            //TODO: avaliar regra !!!

            //var drone = _droneRepository.ObterTodos().Result.FirstOrDefault(d => d.Capacidade >= pedido.Peso);

            //if (drone == null)
            //    return "Pedido acima do peso máximo aceito.";

            //double distancia = _localizacaoLoja.CalcularDistanciaEmKilometros(new Localizacao(pedido.Cliente.Latitude, pedido.Cliente.Longitude));
            //distancia *= 2;

            //int tempoTrajetoCompleto = _localizacaoLoja.CalcularTempoTrajetoEmMinutos(distancia, drone.Velocidade);

            //if (tempoTrajetoCompleto > drone.Autonomia)
            //    return "Fora da área de entrega.";

            var pedido = new Pedido(message.Peso, message.DataHora, message.Status, message.Valor);
            pedido.InformarCliente(user.Cliente);

            pedido.AdicionarEvento(new PedidoAdicionadoEvent(pedido.Id, pedido.Valor,message.BandeiraCartao,message.NumeroCartao,message.MesVencimentoCartao,message.AnoVencimentoCartao ));
            await _pedidoRepository.Adicionar(pedido);
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AtualizarSituacaoPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);
            if (pedido == null) return false;

            pedido.AtualizarStatus(message.StatusPedido);
            await _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }
        public async Task<bool> Handle(DespacharPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;


            var pedidos = _pedidoQueries.ObterPedidosEmAberto().Result.ToList();
            if (!pedidos.Any())
                return false;

            var dronesDisponiveis = _droneItinerarioQueries.ObterDronesDisponiveis().Result.ToList();
            if (!dronesDisponiveis.Any())
                return false;

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
                        localizacaoOrigem = message.LocalizacaoLoja;


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

                    if (!pedidosEntregar.Any()) continue;


                    await _mediatr.EnviarComando(new AtualizarAutonomiaDroneCommand(drone.Id, autonomiaDisponivel));
                    await _mediatr.EnviarComando(new AdicionarDroneItinerarioCommand(DateTime.Now, drone.Id, EnumStatusDrone.EmTransito));

                    foreach (var pedidoentregar in pedidosEntregar)
                    {
                        var pedidoDespachado = await _pedidoRepository.ObterPorId(pedidoentregar.Id);
                        if (pedido == null) return false;

                        var entregador = await _droneRepository.ObterPorId(drone.Id);
                        if (entregador == null) return false;

                        pedidoDespachado.Drone = entregador;
                        pedidoDespachado.InformarStatus(EnumStatusPedido.EmTransito);
                        pedidoDespachado.AdicionarEvento(new PedidoDespachadoEvent(pedido.Id));
                        await _pedidoRepository.Atualizar(pedidoDespachado);
                        await _pedidoRepository.UnitOfWork.Commit();
                        pedidos.Remove(pedidoentregar);
                    }
                }



            }

            return true;
        }

        private bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
                _mediatr.PublicarNotificacao(new DomainNotification(message.MessageType, error.ErrorMessage));
            }

            return false;
        }
    }
}
