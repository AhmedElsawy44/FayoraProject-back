using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Domain.Entities.ReviewModule;
using Fayora.Domain.Enums.ReviewModule;
using Microsoft.EntityFrameworkCore;

namespace Fayora.Infrastructure.Persistence.Repositories.ReviewModule;

public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public void AddReview(Review review)
    {
        context.Set<Review>().Add(review);
    }

    public async Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken ct = default)
    {
        return await context.Set<Review>()
            .FirstOrDefaultAsync(r => r.Id == reviewId, ct);
    }

    public async Task<bool> HasReviewForBookingAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await context.Set<Review>()
            .AnyAsync(r => r.BookingId == bookingId && !r.IsDeleted, ct);
    }

    public async Task<List<Review>> GetByTargetAsync(
        Guid targetId,
        ReviewTargetType type,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        return await context.Set<Review>()
            .Include(r => r.Reviewer)
            .Where(r => r.TargetId == targetId && r.TargetType == type && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<List<Review>> GetMyReviewsAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Set<Review>()
            .Include(r => r.Reviewer)
            .Where(r => r.ReviewerId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public void AddReport(ReviewReport report)
    {
        context.Set<ReviewReport>().Add(report);
    }

    public async Task<bool> HasReportByUserAsync(Guid reviewId, Guid userId, CancellationToken ct = default)
    {
        return await context.Set<ReviewReport>()
            .AnyAsync(rr => rr.ReviewId == reviewId && rr.ReporterId == userId, ct);
    }

    public async Task<List<ReviewReport>> GetPendingReportsAsync(CancellationToken ct = default)
    {
        return await context.Set<ReviewReport>()
            .Where(rr => !rr.IsResolved)
            .OrderByDescending(rr => rr.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<ReviewReport?> GetReportByIdAsync(Guid reportId, CancellationToken ct = default)
    {
        return await context.Set<ReviewReport>()
            .FirstOrDefaultAsync(rr => rr.Id == reportId, ct);
    }
}
