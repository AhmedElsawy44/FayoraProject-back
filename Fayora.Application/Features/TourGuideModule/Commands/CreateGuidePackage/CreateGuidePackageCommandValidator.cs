using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage
{
    public class CreateGuidePackageCommandValidator : AbstractValidator<CreateGuidePackageCommand>
    {
        public CreateGuidePackageCommandValidator()
        {
        }
    }
}
