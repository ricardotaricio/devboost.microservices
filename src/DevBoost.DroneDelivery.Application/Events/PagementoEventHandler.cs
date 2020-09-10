using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.Commands;
using DevBoost.DroneDelivery.Core.Domain.Enumerators;
using DevBoost.DroneDelivery.Core.Domain.Interfaces.Handlers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DevBoost.DroneDelivery.Application.Events
{

    public class PagementoEventHandler : INotificationHandler<PagementoPedidoProcessadoEvent>
    {
        private readonly IMediatrHandler _mediatr;

        public PagementoEventHandler(IMediatrHandler mediatr)
        {
            _mediatr = mediatr;
        }

        public async Task Handle(PagementoPedidoProcessadoEvent message, CancellationToken cancellationToken)
        {
            if (message.SituacaoPagamento == SituacaoPagamento.Aguardando) return;

            var statusPedido = message.SituacaoPagamento == SituacaoPagamento.Autorizado ? EnumStatusPedido.AguardandoEntregador : EnumStatusPedido.PagamentoRejeitado;

            await _mediatr.EnviarComando(new AtualizarSituacaoPedidoCommand(message.EntityId, statusPedido));
        }
    }
}
