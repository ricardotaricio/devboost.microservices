using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.Validations;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class AdicionarPedidoCommand : Command
    {
        public AdicionarPedidoCommand(int peso, double valor)
        {
            Peso = peso;
            DataHora = System.DateTime.Now;
            Status = EnumStatusPedido.AguardandoPagamento;
            Valor = valor;

        }


        public double Valor { get; set; }
        public int Peso { get; private set; }
        public DateTime DataHora { get; private set; }
        public EnumStatusPedido Status { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarPedidoValidation().Validate(this);
            return ValidationResult.IsValid;
        }

    }




}
