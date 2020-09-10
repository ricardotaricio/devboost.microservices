using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace DevBoost.DroneDelivery.Application.Validations
{
    [ExcludeFromCodeCoverage]
    public class AdicionarDroneValidation : AbstractValidator<AdicionarDroneCommand>
    {

        public AdicionarDroneValidation()
        {

            RuleFor(p => p.Autonomia)
                .GreaterThan(0)
                .WithMessage("A autonomia deve ser maior que zero");

            RuleFor(p => p.Capacidade)
                .GreaterThan(0)
                .WithMessage("A capacidade deve ser maior que zero")
            .LessThanOrEqualTo(12)
                .WithMessage("A Capacidade deve ser menor ou igual a 12");

            RuleFor(p => p.Carga)
                .GreaterThan(0)
                .WithMessage("A carga deve ser maior que zero");

            RuleFor(p => p.Velocidade)
                .GreaterThan(0)
                .WithMessage("A velocidade deve ser maior que zero");
        }

    }
}
