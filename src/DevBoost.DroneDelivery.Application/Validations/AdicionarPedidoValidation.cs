using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;

namespace DevBoost.DroneDelivery.Application.Validations
{
    public class AdicionarPedidoValidation : AbstractValidator<AdicionarPedidoCommand>
    {

        public AdicionarPedidoValidation()
        {
            RuleFor(p => p.Peso)
                .GreaterThan(0)
                .WithMessage("Peso do pedido deve ser maior que zero");

            RuleFor(p => p.Peso)
                .LessThanOrEqualTo(12) 
                .WithMessage($"O Peso tem que ser menor ou igual a 12 KGs"); //TODO: Implementar classe utils ou coletar da capacidade do Drone

        }

    }
}
