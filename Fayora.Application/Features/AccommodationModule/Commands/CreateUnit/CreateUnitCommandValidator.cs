using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnit;

public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
    }
}
