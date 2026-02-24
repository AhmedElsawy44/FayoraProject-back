using ErrorOr;
using Fayora.Domain.Common;
using Fayora.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fayora.Domain.Entitties.Identity
{
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

        public static ErrorOr<VerificationRequest> Create(Guid userId, RequestType requestType, List<(DocumentType Type, string Url)> uploadedDocuments)
        {
            if (requestType == RequestType.TourGuide)
            {
                var requiredDocsForGuide = new[]
                {
                    DocumentType.NationalId,
                    DocumentType.TourGuideLicense,
                    DocumentType.CriminalRecord
                };

                var uploadedTypes = uploadedDocuments.Select(d => d.Type).ToList();
                var missingDocs = requiredDocsForGuide.Except(uploadedTypes).ToList();

                if (missingDocs.Any())
                {
                    string missingNames = string.Join(", ", missingDocs);
                    return Error.Validation(
                        code: "Verification.MissingDocuments",
                        description: $"Cannot create Tour Guide request. Missing required documents: {missingNames}");
                }
            }

            var request = new VerificationRequest(userId, requestType);

            foreach (var doc in uploadedDocuments)
            {
                request._verificationDocuments.Add(new VerificationDocument(doc.Type, doc.Url));
            }

            return request;
        }

        public ErrorOr<Success> ReviewRequest(Guid adminId, RequestStatus newStatus, string? adminComment = null)
        {
            if (!_verificationDocuments.Any())
                return Error.Validation("Request.NoDocuments", "Cannot review a request with no documents.");

            ReviewedBy = adminId;
            RequestStatus = newStatus;
            AdminComment = adminComment;
            ReviewedAt = DateTimeOffset.UtcNow;

            Updated();
            return Result.Success;
        }

        public ErrorOr<Success> AddVerificationDocument(DocumentType documentType, string documentUrl)
        {
            if (RequestStatus != RequestStatus.Pending)
                return Error.Validation("Request.Closed", "Cannot add documents to a closed request.");

            var document = new VerificationDocument(documentType, documentUrl);
            _verificationDocuments.Add(document);
            Updated();
            return Result.Success;
        }

        private VerificationRequest() { }
    }
}