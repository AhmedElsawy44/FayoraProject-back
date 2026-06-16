using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.MasterAmenities.Commands.Toggle;

public class ToggleMasterAmenityCommandHandler(
    IUnitOfWork unitOfWork,
    IMasterAmenityRepository amenityRepository
) : ICommandHandler<ToggleMasterAmenityCommand, Result<Unit>>
{

    public async Task<Result<Unit>> Handle(ToggleMasterAmenityCommand request, CancellationToken cancellationToken)
    {
        var amenity = await amenityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (amenity is null)
        {
            return Error.NotFound("MasterAmenity.NotFound", $"Master amenity with ID '{request.Id}' was not found.");
        }

        amenity.ToggleStatus();

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}