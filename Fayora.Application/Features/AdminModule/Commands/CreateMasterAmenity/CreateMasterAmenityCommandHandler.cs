using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.AccommodationModule;

namespace Fayora.Application.Features.AdminModule.Commands.CreateMasterAmenity;

public class CreateMasterAmenityCommandHandler(
    IMasterAmenityRepository masterAmenityRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMasterAmenityCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateMasterAmenityCommand request, CancellationToken cancellationToken)
    {
        var amenityResult = MasterAmenity.Create(
            request.Name,
            request.Category,
            request.IconUrl);

        if (amenityResult.IsError)
        {
            return amenityResult.Errors;
        }

        masterAmenityRepository.Add(amenityResult.Value);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return amenityResult.Value.Id;
    }
}