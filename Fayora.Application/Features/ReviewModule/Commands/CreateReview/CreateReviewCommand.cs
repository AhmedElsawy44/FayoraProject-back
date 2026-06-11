using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Commands.CreateReview;

public record CreateReviewCommand(Guid BookingId, decimal Rating, string? Comment) : ICommand<Result<Guid>>;
