using Fayora.Domain.Entities.ReviewModule;
using Fayora.Domain.Enums.ReviewModule;

namespace Fayora.Application.Common.Interfaces.Persistences.ReviewModule;

public interface IReviewRepository
{
    void AddReview(Review review);
    Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken ct = default);
    Task<bool> HasReviewForBookingAsync(Guid bookingId, CancellationToken ct = default);
    Task<List<Review>> GetByTargetAsync(Guid targetId, ReviewTargetType type, int page, int pageSize, CancellationToken ct = default);
    Task<List<Review>> GetMyReviewsAsync(Guid userId, CancellationToken ct = default);
    void AddReport(ReviewReport report);
    Task<bool> HasReportByUserAsync(Guid reviewId, Guid userId, CancellationToken ct = default);
    Task<List<ReviewReport>> GetPendingReportsAsync(CancellationToken ct = default);
    Task<ReviewReport?> GetReportByIdAsync(Guid reportId, CancellationToken ct = default);
}
