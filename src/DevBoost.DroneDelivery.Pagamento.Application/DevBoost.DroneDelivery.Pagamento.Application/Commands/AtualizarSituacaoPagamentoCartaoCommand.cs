using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Pagamento.Application.Validations;
using DevBoost.DroneDelivery.Pagamento.Domain.Enumerators;
using DevBoost.DroneDelivery.Pagamento.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBoost.DroneDelivery.Pagamento.Application.Commands
{
    public class AtualizarSituacaoPagamentoCartaoCommand : Command
    {
        public AtualizarSituacaoPagamentoCartaoCommand(Guid pagamentoId, SituacaoPagamento situacaoPagamneto, Guid pedidoId, double valor)
        {
            PagamentoId = pagamentoId;
            SituacaoPagamneto = situacaoPagamneto;
            PedidoId = pedidoId;
            Valor = valor;
        }

        public Guid PagamentoId { get; private set; }
        public SituacaoPagamento SituacaoPagamneto { get; private set; }
        public Guid PedidoId { get; private set; }
        public double Valor { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new AtualizarSituacaoPagamentoCartaoValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
