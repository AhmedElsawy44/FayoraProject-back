using Fayora.Domain.Enums.SharedModule;

namespace Fayora.Domain.Entities.IdentityModule;

public class VerificationDocument : BaseEntity<int>
{
    public int RequestId { get; init; }
    public Guid? ReviewedBy { get; private set; }
    public DocumentType DocumentType { get; init; }
    public string DocumentUrl { get; init; } = string.Empty;
    public DateOnly? ExpireDate { get; private set; }
    public DateTimeOffset UploadAt { get; init; }


    internal VerificationDocument(DocumentType documentType, string documentUrl)
    {
        DocumentType = documentType;
        DocumentUrl = documentUrl;
        UploadAt = DateTimeOffset.UtcNow;
    }

    internal void ApproveDocument(Guid adminId, DateOnly? expireDate = null)
    {
        ReviewedBy = adminId;
        ExpireDate = expireDate;
    }

    internal void RejectDocument(Guid adminId, string rejectionReason)
    {
        ReviewedBy = adminId;
    }

    private VerificationDocument() { }
}