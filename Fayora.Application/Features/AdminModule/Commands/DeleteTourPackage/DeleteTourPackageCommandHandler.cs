using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.AdminModule;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteTourPackage;

public class DeleteTourPackageCommandHandler(IAdminRepository adminRepository)
    : ICommandHandler<DeleteTourPackageCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteTourPackageCommand request, CancellationToken cancellationToken)
    {
        var result = await adminRepository.DeleteTourPackageAsync(request.Id, cancellationToken);

        if (!result)
        {
            return TourGuideErrors.PackageNotFound;
        }

        return Result.Success;
    }
}
