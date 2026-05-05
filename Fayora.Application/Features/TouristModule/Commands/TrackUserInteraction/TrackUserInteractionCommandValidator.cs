using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction
{
    public class TrackUserInteractionCommandValidator : AbstractValidator<TrackUserInteractionCommand>
    {
        public TrackUserInteractionCommandValidator()
        {
            RuleFor(x => x.EntityId)
                .NotEmpty()
                .WithMessage("EntityId is required.");

            RuleFor(x => x.EntityType)
                .IsInEnum()
                .WithMessage("Invalid Entity Type value.");

            RuleFor(x => x.InteractionType)
                .IsInEnum()
                .WithMessage("Invalid Interaction Type value.");
        }
    }
}
