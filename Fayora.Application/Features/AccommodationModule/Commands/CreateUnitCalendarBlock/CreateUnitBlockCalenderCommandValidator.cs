using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AccommodationModule.Commands.CreateUnitCalendarBlock
{
    public class CreateUnitBlockCalenderCommandValidator : AbstractValidator<CreateUnitCalendarBlockCommand>
    {
        public CreateUnitBlockCalenderCommandValidator()
        {
        }
    }
}
