using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.AccommodationModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteCompany;

public class DeleteCompanyCommandHandler(IAdminRepository adminRepository)
    : ICommandHandler<DeleteCompanyCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var result = await adminRepository.DeleteCompanyAsync(request.Id, cancellationToken);

        if (!result)
        {
            return AccommodationErrors.UserNotFound;
        }

        return Result.Success;
    }
}
