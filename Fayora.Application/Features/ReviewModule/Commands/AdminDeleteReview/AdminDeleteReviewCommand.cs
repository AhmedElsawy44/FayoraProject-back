using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Commands.AdminDeleteReview;

public record AdminDeleteReviewCommand(Guid ReviewId) : ICommand<Result<Success>>;
