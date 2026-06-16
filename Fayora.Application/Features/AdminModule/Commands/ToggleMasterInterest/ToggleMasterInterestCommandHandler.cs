using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.ToggleMasterInterest;

public class ToggleMasterInterestCommandHandler(
    IMasterInterestRepository masterInterestRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ToggleMasterInterestCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(ToggleMasterInterestCommand request, CancellationToken cancellationToken)
    {
        var interest = await masterInterestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (interest is null)
            return Error.NotFound("MasterInterest.NotFound", "Master interest not found.");

        interest.IsActive = !interest.IsActive;
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}

