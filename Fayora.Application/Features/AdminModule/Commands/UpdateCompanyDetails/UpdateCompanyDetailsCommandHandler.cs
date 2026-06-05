using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.UpdateCompanyDetails;

public class UpdateCompanyDetailsCommandHandler(IAdminRepository adminRepository)
    : ICommandHandler<UpdateCompanyDetailsCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(
        UpdateCompanyDetailsCommand request,
        CancellationToken cancellationToken)
    {
        var result = await adminRepository.UpdateCompanyDetailsAsync(request.Id, request.Request, cancellationToken);

        if (!result)
        {
            return AccommodationErrors.UserNotFound;
        }

        return Result.Success;
    }
}
