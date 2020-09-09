using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Pagamento.Application.Events
{
    public class AtualizarSituacaoPedidoEvent : Event
    {
        public AtualizarSituacaoPedidoEvent(Guid entityId, Guid pagamentoId, SituacaoPagamento situacaoPagamento) : base(entityId)
        {
            PagamentoId = pagamentoId;
            SituacaoPagamento = situacaoPagamento;
        }

        public Guid PagamentoId { get; set; }
        public SituacaoPagamento SituacaoPagamento { get; set; }
    }
}
