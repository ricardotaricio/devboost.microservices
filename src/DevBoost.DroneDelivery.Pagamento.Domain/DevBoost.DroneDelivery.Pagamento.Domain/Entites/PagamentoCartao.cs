using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using DevBoost.DroneDelivery.Pagamento.Domain.ValueObjects;
using System;

namespace DevBoost.DroneDelivery.Pagamento.Domain.Entites
{
    public class PagamentoCartao
    {
        public Guid Id { get; set; }
        public Guid PedidoId { get; set; }
        public double Valor { get; set; }
        public SituacaoPagamento Situacao { get; set; }

        public Cartao Cartao { get; set; }
    }
}
