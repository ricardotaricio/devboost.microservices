using DevBoost.DroneDelivery.Domain.Interfaces;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarPedidoCommand, bool>, IRequestHandler<AtualizarSituacaoPedidoCommand, bool>
    {
        private readonly IPedidoRepository _repositoryPedido;
        private readonly IUsuarioAutenticado _usuarioAutenticado;
        private readonly IUserRepository _userRepository;
        private readonly IMediatrHandler _mediatr;

        public PedidoCommandHandler(IMediatrHandler mediatr,IPedidoRepository repositoryPedido, IUsuarioAutenticado usuarioAutenticado, IUserRepository userRepository)
        {
            _repositoryPedido = repositoryPedido;
            _usuarioAutenticado = usuarioAutenticado;
            _userRepository = userRepository;
            _mediatr = mediatr;
        }

        public async Task<bool> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var user = await _userRepository.ObterPorNome(_usuarioAutenticado.GetCurrentUserName());
            if (user.Cliente == null)
                return false;

            var pedido = new Pedido(message.Peso, message.DataHora, message.Status, message.Valor);
            pedido.InformarCliente(user.Cliente);

            await _repositoryPedido.Adicionar(pedido);
            return await _repositoryPedido.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AtualizarSituacaoPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var pedido = await _repositoryPedido.ObterPorId(message.PedidoId);
            if (pedido == null) return false;

            pedido.AtualizarStatus(message.StatusPedido);
            await _repositoryPedido.Atualizar(pedido);

            return await _repositoryPedido.UnitOfWork.Commit();
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
