using Fayora.Domain.Enums;
using System;

namespace Fayora.Domain.Entities.Identity;

public class VerificationDocument : BaseEntity<int>
{
    public int RequestId { get; init; }
    public Guid? ReviewedBy { get; private set; }
    public DocumentType DocumentType { get; init; }
    public string DocumentUrl { get; init; } = string.Empty;
    public RequestStatus DocumentStatus { get; private set; }
    public DateOnly? ExpireDate { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset UploadAt { get; init; }

    
    internal VerificationDocument(DocumentType documentType, string documentUrl)
    {
        DocumentType = documentType;
        DocumentUrl = documentUrl;
        DocumentStatus = RequestStatus.Pending;
        UploadAt = DateTimeOffset.UtcNow;
    }

    internal void ApproveDocument(Guid adminId, DateOnly? expireDate = null)
    {
        ReviewedBy = adminId;
        DocumentStatus = RequestStatus.Approved;
        ExpireDate = expireDate;
    }

    internal void RejectDocument(Guid adminId, string rejectionReason)
    {
        ReviewedBy = adminId;
        DocumentStatus = RequestStatus.Rejected;
        RejectionReason = rejectionReason;
    }

    private VerificationDocument() { }
}