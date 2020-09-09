using DevBoost.DroneDelivery.Core.Domain.Entities;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Pagamento.Domain.Entites
{
    [ExcludeFromCodeCoverage]
    public class PagamentoCartao : Entity
    {
        public PagamentoCartao(): base()
        {

        }
        public Guid PedidoId { get; set; }
        public double Valor { get; set; }
        public SituacaoPagamento Situacao { get; set; }

        public Guid CartaoId { get; set; }
        public Cartao  Cartao { get; set; }


    }
}
