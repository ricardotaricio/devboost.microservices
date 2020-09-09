﻿using DevBoost.DroneDelivery.Application.Commands;
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

            RuleFor(c => c.Senha)
             .NotEmpty()
             .WithMessage("Senha é necessária");

        }

    }

}
