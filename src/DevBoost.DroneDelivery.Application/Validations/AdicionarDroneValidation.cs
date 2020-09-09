using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;


namespace DevBoost.DroneDelivery.Application.Validations
{
  public class AdicionarDroneValidation : AbstractValidator<AdicionarDroneCommand>
    {

        public AdicionarDroneValidation()
        {

            RuleFor(p => p.Autonomia)
                .LessThanOrEqualTo(0)
                .WithMessage("A autonomia deve ser maior que zero");

            RuleFor(p => p.Capacidade)
                .LessThanOrEqualTo(0)
                .WithMessage("A capacidade deve ser maior que zero");

            RuleFor(p => p.Carga)
                .LessThanOrEqualTo(0)
                .WithMessage("A carga deve ser maior que zero");

            RuleFor(p => p.Velocidade)
                .LessThanOrEqualTo(0)
                .WithMessage("A velocidade deve ser maior que zero");
        }

    }
}
