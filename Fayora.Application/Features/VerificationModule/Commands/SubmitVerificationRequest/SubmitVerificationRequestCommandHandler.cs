using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.SharedModule;
using MediatR;

namespace Fayora.Application.Features.VerificationModule.Commands.SubmitVerificationRequest
{

    public class SubmitVerificationRequestCommandHandler(
    IFileStorageService fileStorage,
    IVerificationRepository verificationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitVerificationRequestCommand, Result<SubmitVerificationRequestResponse>>
    {
        public async Task<Result<SubmitVerificationRequestResponse>> Handle(
            SubmitVerificationRequestCommand command,
            CancellationToken ct)
        {
            // Check if there's an existing pending request of the same type for the user
            var existing = await verificationRepository.GetByUserIdAndTypeAsync(
                command.UserId, command.RequestType, ct);

            if (existing is not null && existing.RequestStatus == RequestStatus.Pending)
                return Error.Conflict(
                    code: "Verification.AlreadyExists",
                    description: "You already have a pending verification request.");

            // upload files to storage and get URLs
            var folderName = command.RequestType switch
            {
                RequestType.TourGuide => "guides",
                RequestType.HousingUnit => "owners",
                RequestType.TourCompany => "companies",
                _ => "documents"
            };

            var uploadedDocuments = new List<(DocumentType Type, string Url)>();

            foreach (var (docType, file) in command.Documents)
            {
                var url = await fileStorage.SaveFileAsync(
                    file.OpenReadStream(),
                    file.FileName,
                    folderName);

                uploadedDocuments.Add((docType, url));
            }

            //  Make a VerificationRequest
            var result = VerificationRequest.Create(
                command.UserId,
                command.RequestType,
                uploadedDocuments);

            if (result.IsError)
                return result.Errors;

            // Save In DB
            await verificationRepository.AddAsync(result.Value, ct);
            await unitOfWork.CommitChangesAsync(ct);

            return new SubmitVerificationRequestResponse(
                VerificationRequestId: result.Value.Id,
                RequestType: result.Value.RequestType.ToString(),
                Status: result.Value.RequestStatus,
                Message: "Your request has been submitted successfully.",
                SubmittedAt: result.Value.CreatedAt
                    //Documents: result.Value.VerificationDocuments
                    //    .Select(d => new DocumentResponseDto(
                    //        DocumentType: d.DocumentType.ToString(),
                    //        Status: d.DocumentStatus))
                    //    .ToList()
                    );
        }
    }

}
