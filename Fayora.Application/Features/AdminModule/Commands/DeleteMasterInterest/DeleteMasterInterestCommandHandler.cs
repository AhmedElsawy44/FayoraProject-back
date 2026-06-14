using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteMasterInterest;

public class DeleteMasterInterestCommandHandler(
    IMasterInterestRepository masterInterestRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteMasterInterestCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteMasterInterestCommand request, CancellationToken cancellationToken)
    {
        var interest = await masterInterestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (interest is null)
            return Error.NotFound("MasterInterest.NotFound", "Master interest not found.");

        masterInterestRepository.Remove(interest);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }
}

