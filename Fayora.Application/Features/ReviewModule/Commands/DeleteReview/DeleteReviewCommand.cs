using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Commands.DeleteReview;

public record DeleteReviewCommand(Guid ReviewId) : ICommand<Result<Success>>;
