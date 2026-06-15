using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Features.AdminModule.Commands.Delete;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteMasterAmenity;

public class DeleteMasterAmenityCommandHandler(
    IUnitOfWork unitOfWork,
    IMasterAmenityRepository amenityRepository
) : ICommandHandler<DeleteMasterAmenityCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteMasterAmenityCommand request, CancellationToken cancellationToken)
    {
        var amenity = await amenityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (amenity is null)
        {
            return Error.NotFound("MasterAmenity.NotFound", $"Master amenity with ID '{request.Id}' was not found.");
        }

        amenityRepository.Delete(amenity);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}