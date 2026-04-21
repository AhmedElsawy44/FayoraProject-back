using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourCompanyModule.Commands.CreateCompanyPackage
{

    public class CreateCompanyPackageCommandValidator : AbstractValidator<CreateCompanyPackageCommand>
    {
        public CreateCompanyPackageCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.DurationHours)
                .GreaterThan(0).WithMessage("Duration must be greater than zero.");

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0).WithMessage("Max capacity must be greater than zero.");

            RuleFor(x => x.AdultPrice)
                .GreaterThan(0).WithMessage("Adult price must be greater than zero.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");
        }

    }
    
}

