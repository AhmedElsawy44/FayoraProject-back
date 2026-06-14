using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.TouristModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Features.AdminModule.Commands.CreateMasterInterest;

public class CreateMasterInterestCommandHandler(
    IMasterInterestRepository masterInterestRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateMasterInterestCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateMasterInterestCommand request, CancellationToken cancellationToken)
    {
        var interest = new MasterInterest(request.Code, request.Name, request.IconUrl, request.SortOrder);

        masterInterestRepository.Add(interest);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return interest.Id;
    }
}

