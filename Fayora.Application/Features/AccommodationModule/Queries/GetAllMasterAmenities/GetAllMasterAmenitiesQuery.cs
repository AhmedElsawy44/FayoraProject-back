using MediatR;

namespace Fayora.Application.Features.AccommodationModule.Queries.GetAllMasterAmenities;

public record GetAllMasterAmenitiesQuery : IRequest<GetAllMasterAmenitiesResult>;
