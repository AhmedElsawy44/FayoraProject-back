using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.UpdateGuideDetails;

public class UpdateGuideDetailsCommandHandler(IAdminRepository adminRepository)
    : ICommandHandler<UpdateGuideDetailsCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        UpdateGuideDetailsCommand request,
        CancellationToken cancellationToken)
    {
        var result = await adminRepository.UpdateGuideDetailsAsync(request.Id, request.Request, cancellationToken);

        if (!result)
        {
            return AccommodationErrors.UserNotFound;
        }

        return Result.Success;
    }
}
