using DevBoost.DroneDelivery.Application.Commands;
using FluentValidation;

namespace DevBoost.DroneDelivery.Application.Validations
{
    public class AdicionarUsuarioValidation : AbstractValidator<AdicionarUsuarioCommand>
    {
        public AdicionarUsuarioValidation()
        {
            RuleFor(c => c.UserName)
              .NotEmpty()
              .WithMessage("Nome é necessário");

            RuleFor(c => c.Password)
             .NotEmpty()
             .WithMessage("Senha é necessária");
        }
    }
}
