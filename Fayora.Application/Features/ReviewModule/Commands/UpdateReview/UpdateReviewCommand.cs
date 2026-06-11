using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Commands.UpdateReview;

public record UpdateReviewCommand(Guid ReviewId, decimal Rating, string? Comment) : ICommand<Result<Success>>;
