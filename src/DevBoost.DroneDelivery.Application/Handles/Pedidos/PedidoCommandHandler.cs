using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Domain.Interfaces;
using DevBoost.DroneDelivery.Domain.Interfaces.Repositories;
using DevBoost.DroneDelivery.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Handles.Pedidos
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarPedidoCommand, bool>, IRequestHandler<ValidarAutorizacaoPagamentoCommand, bool>
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

        public async Task<bool> Handle(ValidarAutorizacaoPagamentoCommand request, CancellationToken cancellationToken)
        {
            await _repositoryPedido.Atualizar(request.Pedido);
            return await _repositoryPedido.UnitOfWork.Commit();
        }
    }
}
