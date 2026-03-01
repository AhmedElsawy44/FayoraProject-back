using Fayora.Domain.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;

namespace Fayora.Domain.Entities.Identity;

public class VerificationRequest : AuditableEntity<int>
{
    public Guid UserId { get; init; }
    public Guid? ReviewedBy { get; private set; }
    public RequestType RequestType { get; init; }
    public RequestStatus RequestStatus { get; private set; }
    public string? AdminComment { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }

    private readonly List<VerificationDocument> _verificationDocuments = new();
    public IReadOnlyCollection<VerificationDocument> VerificationDocuments => _verificationDocuments.AsReadOnly();

    private VerificationRequest(Guid userId, RequestType requestType)
    {
        UserId = userId;
        RequestType = requestType;
        RequestStatus = RequestStatus.Pending;
    }

    public static Result<VerificationRequest> Create(Guid userId, RequestType requestType, List<(DocumentType Type, string Url)> uploadedDocuments)
    {
        List<DocumentType> requiredDocs = requestType switch
        {
            RequestType.TourGuide => new List<DocumentType>
            {
                DocumentType.NationalId,
                DocumentType.TourGuideLicense,
                DocumentType.CriminalRecord
            },
            RequestType.HousingUnit => new List<DocumentType>
            {
                DocumentType.NationalId,
                DocumentType.PropertyOwnership
            },
            _ => throw new Exception()
        };


        var uploadedTypes = uploadedDocuments.Select(d => d.Type).ToList();
        var missingDocs = requiredDocs.Except(uploadedTypes).ToList();

        if (missingDocs.Any())
        {
            string missingNames = string.Join(", ", missingDocs);
            return Error.Validation(
                code: "Verification.MissingDocuments",
                description: $"Cannot create {requestType} request. Missing required documents: {missingNames}");
        }

        var request = new VerificationRequest(userId, requestType);

        foreach (var doc in uploadedDocuments)
        {
            request._verificationDocuments.Add(new VerificationDocument(doc.Type, doc.Url));
        }

        return request;
    }

    public void ReviewRequest(Guid adminId, RequestStatus newStatus, string? adminComment = null)
    {
        ReviewedBy = adminId;
        RequestStatus = newStatus;
        AdminComment = adminComment;
        ReviewedAt = DateTimeOffset.UtcNow;
        Updated();
    }

    public Result<Success> AddVerificationDocument(DocumentType documentType, string documentUrl)
    {
        if (RequestStatus != RequestStatus.Pending && RequestStatus != RequestStatus.Rejected)
            return Error.Validation("Request.Closed", "Cannot add documents to a closed request.");

        var document = new VerificationDocument(documentType, documentUrl);
        _verificationDocuments.Add(document);
        Updated();
        return Result.Success;
    }

    public void RemoveVerificationDocument(int documentId)
    {
        var document = _verificationDocuments.FirstOrDefault(d => d.Id == documentId);
        if (document != null)
        {
            _verificationDocuments.Remove(document);
            Updated();
        }
    }

    private VerificationRequest() { }
}