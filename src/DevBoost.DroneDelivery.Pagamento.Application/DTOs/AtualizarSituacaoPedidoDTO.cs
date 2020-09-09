using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DevBoost.DroneDelivery.Pagamento.Application.DTOs
{
    [ExcludeFromCodeCoverage]
    public class AtualizarSituacaoPedidoDTO
    {
        public Guid PedidoId { get; set; }
        public Guid PagamentoId { get; set; }
        public SituacaoPagamento SituacaoPagamento { get; set; }
    }
}
