using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetTourGuideById;

public record GetTourGuideByIdQuery(Guid TourGuideId) : IRequest<Result<GetTourGuideByIdResult>>;
