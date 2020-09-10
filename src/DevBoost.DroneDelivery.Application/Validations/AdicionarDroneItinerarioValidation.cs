using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;

namespace DevBoost.DroneDelivery.Application.Validations
{
    public class AdicionarDroneItinerarioValidation : AbstractValidator<AdicionarDroneItinerarioCommand>
    {
        public AdicionarDroneItinerarioValidation()
        {
            RuleFor(p => p.DroneId)
              .NotEmpty()
                .WithMessage("Drone deve ser informado!");
        }
    }
}
