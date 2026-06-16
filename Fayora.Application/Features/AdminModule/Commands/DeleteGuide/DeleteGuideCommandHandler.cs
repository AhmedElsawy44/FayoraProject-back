using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteGuide;

public class DeleteGuideCommandHandler(IAdminRepository adminRepository)
    : ICommandHandler<DeleteGuideCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteGuideCommand request, CancellationToken cancellationToken)
    {
        var result = await adminRepository.DeleteGuideAsync(request.Id, cancellationToken);

        if (!result)
        {
            return AccommodationErrors.UserNotFound;
        }

        return Result.Success;
    }
}
