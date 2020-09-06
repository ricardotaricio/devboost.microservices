using DevBoost.DroneDelivery.Core.Domain.Entities;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using DevBoost.DroneDelivery.Pagamento.Domain.ValueObjects;
using System;

namespace DevBoost.DroneDelivery.Pagamento.Domain.Entites
{
    public class PagamentoCartao : Entity
    {
        public PagamentoCartao(): base()
        {

        }

        public PagamentoCartao(Guid id): base(id)
        {

        }

        public Guid PedidoId { get; set; }
        public double Valor { get; set; }
        public SituacaoPagamento Situacao { get; set; }

        public Cartao Cartao { get; set; }
    }
}
