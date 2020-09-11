using DevBoost.Dronedelivery.Domain.Enumerators;
using DevBoost.DroneDelivery.Application.Validations;
using DevBoost.DroneDelivery.Core.Domain.Messages;
using System;

namespace DevBoost.DroneDelivery.Application.Commands
{
    public class DespacharPedidoCommand : Command
    {
        public DespacharPedidoCommand(Guid droneId, Guid pedidoId, EnumStatusPedido statusPedido)
        {
            DroneId = droneId;
            PedidoId = pedidoId;
            StatusPedido = statusPedido;
        }

        public Guid DroneId { get; set; }
        public Guid PedidoId { get; set; }
        public EnumStatusPedido  StatusPedido { get; set; }
        public override bool EhValido()
        {
            ValidationResult = new DespacharPedidoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
