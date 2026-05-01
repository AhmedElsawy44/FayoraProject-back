using Fayora.Application.Common.Factories;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Admin.Commands.VerifyContent;

public class VerifyContentCommandHandler(
    IVerificationFactory strategyFactory,
    IUnitOfWork unitOfWork) : IRequestHandler<VerifyContentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(VerifyContentCommand request, CancellationToken ct)
    {
        var strategy = strategyFactory.GetStrategy(request.EntityType);

        if (strategy is null)
            return Error.Validation($"No verification strategy found for entity type: {request.EntityType}");

        var processResult = await strategy.ProcessVerificationAsync(
            request.EntityId,
            request.IsApproved,
            request.AdminNotes,
            ct);

        if (processResult.IsError)
            return processResult;

        await unitOfWork.CommitChangesAsync(ct);

        return Result.Success;
    }
}
