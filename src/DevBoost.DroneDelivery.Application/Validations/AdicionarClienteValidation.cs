using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;

namespace DevBoost.DroneDelivery.Application.Validations
{
    public class AdicionarClienteValidation : AbstractValidator<AdicionarClienteCommand>
    {

        public AdicionarClienteValidation()
        {
            RuleFor(c => c.Nome)
              .NotEmpty()
              .WithMessage("Nome é necessário");

            RuleFor(x => x.Latitude).GreaterThanOrEqualTo(-90).WithMessage("A latitude não pode ser maior que -90");
            RuleFor(x => x.Latitude).LessThanOrEqualTo(90).WithMessage("Latitude não pode ser menor que 90");
            RuleFor(x => x.Longitude).GreaterThanOrEqualTo(-180).WithMessage("A longitude pode ser maior que 180");
            RuleFor(x => x.Longitude).GreaterThan(180).WithMessage("Longitude pode ser menor 180");

        }

    }

}
