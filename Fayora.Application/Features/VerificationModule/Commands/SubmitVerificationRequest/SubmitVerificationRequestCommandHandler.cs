using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.VerificationModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest;

public class SubmitVerificationRequestCommandHandler(
    IVerificationRepository verificationRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider)
    : ICommandHandler<SubmitVerificationRequestCommand, Result<SubmitVerificationRequestResult>>
{
    public async Task<Result<SubmitVerificationRequestResult>> Handle(
        SubmitVerificationRequestCommand command,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var existing = await verificationRepository.GetByUserIdAndTypeAsync(
            userId, command.RequestType, cancellationToken);

        if (existing is not null && existing.RequestStatus == RequestStatus.Pending)
            return VerificationErrors.PendingRequestAlreadyExists;

        var documents = command.Documents
            .Select(d => (d.DocumentType, d.FileUrl))
            .ToList();

        var result = VerificationRequest.Create(
            userId,
            command.RequestType,
            documents);

        if (result.IsError) return result.Errors;

        await verificationRepository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new SubmitVerificationRequestResult(
            result.Value.Id,
            result.Value.RequestType.ToString(),
            result.Value.RequestStatus,
            result.Value.CreatedAt);
    }
}