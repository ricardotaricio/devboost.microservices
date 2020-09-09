using DevBoost.DroneDelivery.Domain.Interfaces;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarPedidoCommand, bool>, IRequestHandler<AtualizarSituacaoPedidoCommand, bool>
    {
        private readonly IPedidoRepository _repositoryPedido;
        private readonly IUsuarioAutenticado _usuarioAutenticado;
        private readonly IUserRepository _userRepository;

        public PedidoCommandHandler(IPedidoRepository repositoryPedido, IUsuarioAutenticado usuarioAutenticado, IUserRepository userRepository)
        {
            _repositoryPedido = repositoryPedido;
            _usuarioAutenticado = usuarioAutenticado;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(AdicionarPedidoCommand request, CancellationToken cancellationToken)
        {
            if (!request.EhValido()) return false;

            var user = await _userRepository.ObterPorNome(_usuarioAutenticado.GetCurrentUserName());
            if (user.Cliente == null)
                return false;

            var pedido = new Pedido(request.Peso, request.DataHora, request.Status, request.Valor);
            pedido.InformarCliente(user.Cliente);

            await _repositoryPedido.Adicionar(pedido);
            return await _repositoryPedido.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AtualizarSituacaoPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return false;

            var pedido = await _repositoryPedido.ObterPorId(message.PedidoId);
            if (pedido == null) return false;

            pedido.AtualizarStatus(message.StatusPedido);
            await _repositoryPedido.Atualizar(pedido);

            return await _repositoryPedido.UnitOfWork.Commit();
        }
    }
}
