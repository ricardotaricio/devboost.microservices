using DevBoost.DroneDelivery.Domain.Interfaces;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Application.Events;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarPedidoCommand, bool>, IRequestHandler<AtualizarSituacaoPedidoCommand, bool>, IRequestHandler<DespacharPedidoCommand,bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IDroneRepository _droneRepository;
        private readonly IUsuarioAutenticado _usuarioAutenticado;
        private readonly IUserRepository _userRepository;
        private readonly IMediatrHandler _mediatr;

        public PedidoCommandHandler(IDroneRepository droneRepository,IMediatrHandler mediatr,IPedidoRepository repositoryPedido, IUsuarioAutenticado usuarioAutenticado, IUserRepository userRepository)
        {
            _pedidoRepository = repositoryPedido;
            _usuarioAutenticado = usuarioAutenticado;
            _userRepository = userRepository;
            _mediatr = mediatr;
            _droneRepository = droneRepository;
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

            var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);
            if (pedido == null) return false;

            var drone = await _droneRepository.ObterPorId(message.DroneId);
            if (drone == null) return false;

            pedido.Drone= drone;
            pedido.InformarStatus(message.StatusPedido);
            pedido.AdicionarEvento(new PedidoDespachadoEvent(pedido.Id));
            await _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
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
