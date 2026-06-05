using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteAccommodation;

public class DeleteAccommodationCommandHandler(IAdminRepository adminRepository)
    : ICommandHandler<DeleteAccommodationCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteAccommodationCommand request, CancellationToken cancellationToken)
    {
        var result = await adminRepository.DeleteAccommodationAsync(request.Id, cancellationToken);

        if (!result)
        {
            return AccommodationErrors.UnitNotFound;
        }

        return Result.Success;
    }
}
