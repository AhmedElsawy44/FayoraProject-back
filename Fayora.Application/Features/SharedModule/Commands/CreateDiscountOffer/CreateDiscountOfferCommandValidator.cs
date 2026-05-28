using Fayora.Domain.Enums.SharedModule;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.SharedModule.Commands.CreateDiscountOffer
{
    public class CreateDiscountOfferCommandValidator : AbstractValidator<CreateDiscountOfferCommand>
    {
        public CreateDiscountOfferCommandValidator()
        {
            RuleFor(x => x.TargetId)
                .NotEmpty().WithMessage("Target ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than zero.")
                .Must((cmd, value) =>
                    cmd.DiscountType != DiscountType.Percentage || value <= 100)
                .WithMessage("Percentage discount cannot exceed 100%.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.")
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("End date must be in the future.");
        }
    }
}
