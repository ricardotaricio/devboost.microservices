using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;

namespace DevBoost.DroneDelivery.Application.Validations
{
    public class DespacharPedidoValidation : AbstractValidator<DespacharPedidoCommand>
    {
        public DespacharPedidoValidation()
        {
            RuleFor(p => p.DroneId)
              .NotEmpty()
              .WithMessage("Drone é necessário");

            RuleFor(p => p.PedidoId)
             .NotEmpty()
             .WithMessage("Pedido é necessária");
        }
    }
}
