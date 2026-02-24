using Fayora.Domain.Enums;


namespace Fayora.Domain.Entitties.Identity
{
    public class VerificationRequest : BaseEntity<int>
    {
        public Guid UserId { get; init; }
        public Guid ReviewedBy { get; init; }
        public RequestType RequestType { get; init; }
        public RequestStatus RequestStatus { get; init; }
        public string? AdminComment { get; init; }
        DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        DateTimeOffset? ReviewedAt { get; init; }

        public VerificationRequest(Guid userId, Guid reviewedBy, RequestType requestType, RequestStatus requestStatus, string? adminComment = null)
        {
            UserId = userId;
            ReviewedBy = reviewedBy;
            RequestType = requestType;
            RequestStatus = requestStatus;
            AdminComment = adminComment;
        }
    }
}
