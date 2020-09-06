using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Pagamento.Application.Events;
using DevBoost.DroneDelivery.Pagamento.Domain.Entites;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using DevBoost.DroneDelivery.Pagamento.Domain.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Pagamento.Application.Commands
{
    public class PagamentoCommandHandler : IRequestHandler<AdicionarPagamentoCartaoCommand, bool>, IRequestHandler<AtualizarSituacaoPagamentoCartaoCommand, bool>
    {
        private readonly IMediatrHandler _bus;
        private readonly IPagamentoRepository _pagamentoRepository;

        public PagamentoCommandHandler(IMediatrHandler bus, IPagamentoRepository pagamentoRepository)
        {
            _bus = bus;
            _pagamentoRepository = pagamentoRepository;
        }

        public async Task<bool> Handle(AdicionarPagamentoCartaoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            PagamentoCartao pagamentoCartao = new PagamentoCartao()
            {
                PedidoId = message.PedidoId,
                Valor = message.Valor,
                Cartao = message.Cartao,
                Situacao = SituacaoPagamento.Aguardando
            };

            await _pagamentoRepository.Adicionar(pagamentoCartao);

            var pagamentoAdicionado = await _pagamentoRepository.UnitOfWork.Commit();

            if (pagamentoAdicionado)
                await _bus.PublicarEvento(new ProcessarPagamentoCartaoEvent(pagamentoCartao.Id));

            return pagamentoAdicionado;
        }

        public async Task<bool> Handle(AtualizarSituacaoPagamentoCartaoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            PagamentoCartao pagamentoCartao = new PagamentoCartao()
            {
                Id = message.PagamentoId,
                Situacao = message.SituacaoPagamneto,
                PedidoId = message.PedidoId,
                Valor = message.Valor
            };

            await _pagamentoRepository.Atualizar(pagamentoCartao);

            var pagamentoAtualizado = await _pagamentoRepository.UnitOfWork.Commit();

            return pagamentoAtualizado;
        }

        private bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
                _bus.PublicarNotificacao(new DomainNotification(message.MessageType, error.ErrorMessage));
            }

            return false;
        }

    }
}
