using DevBoost.DroneDelivery.Core.Domain.Messages;
using DevBoost.DroneDelivery.Pagamento.Application.Validations;
using DevBoost.DroneDelivery.Pagamento.Domain.ValueObjects;
using System;

namespace DevBoost.DroneDelivery.Pagamento.Application.Commands
{
    public class AdicionarPagamentoCartaoCommand : Command
    {
        public AdicionarPagamentoCartaoCommand(Guid pedidoId, double valor, Cartao cartao)
        {
            PedidoId = pedidoId;
            Valor = valor;
            Cartao = cartao;
        }

        public Guid PedidoId { get; private set; }
        public double Valor { get; private set; }
        public Cartao Cartao { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarPagamentoCartaoValidation().Validate(this);

            return ValidationResult.IsValid;
        }
    }
}
