using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Domain.Entities.ReviewModule;
using Fayora.Domain.Enums.ReviewModule;
using Fayora.Application.Features.ReviewModule.Queries.GetPendingReports;
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

    public async Task<List<ReviewReportDto>> GetPendingReportsWithDetailsAsync(CancellationToken ct = default)
    {
        var reports = await context.Set<ReviewReport>()
            .Where(rr => !rr.IsResolved)
            .OrderByDescending(rr => rr.CreatedAt)
            .ToListAsync(ct);

        if (!reports.Any())
        {
            return new List<ReviewReportDto>();
        }

        var reviewIds = reports.Select(r => r.ReviewId).Distinct().ToList();

        // Get reviews with their reviewers
        var reviews = await context.Set<Review>()
            .Include(r => r.Reviewer)
            .Where(r => reviewIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r, ct);

        // Get all unique targets across all reviews to retrieve their names
        var targetIds = reviews.Values.Select(r => r.TargetId).Distinct().ToList();

        // Load HousingUnit names
        var housingUnits = await context.Set<Fayora.Domain.Entities.AccommodationModule.HousingUnit>()
            .Where(hu => targetIds.Contains(hu.Id))
            .ToDictionaryAsync(hu => hu.Id, hu => hu.Title, ct);

        // Load GuidePackage names
        var guidePackages = await context.Set<Fayora.Domain.Entities.GuideModule.GuidePackage>()
            .Where(gp => targetIds.Contains(gp.Id))
            .ToDictionaryAsync(gp => gp.Id, gp => gp.Title, ct);

        // Load TourGuide (User) names
        var tourGuides = await context.Set<Fayora.Domain.Entities.IdentityModule.User>()
            .Where(u => targetIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FirstName + " " + u.LastName, ct);

        var dtos = new List<ReviewReportDto>();
        foreach (var report in reports)
        {
            if (!reviews.TryGetValue(report.ReviewId, out var review))
            {
                continue;
            }

            var authorName = review.Reviewer != null
                ? (review.Reviewer.FirstName + " " + review.Reviewer.LastName)
                : "Unknown User";

            // Resolve target name
            string targetName = "Unknown Target";
            string targetTypeStr = review.TargetType.ToString();

            if (review.TargetType == ReviewTargetType.HousingUnit && housingUnits.TryGetValue(review.TargetId, out var huTitle))
            {
                targetName = huTitle;
            }
            else if (review.TargetType == ReviewTargetType.GuidePackage && guidePackages.TryGetValue(review.TargetId, out var gpTitle))
            {
                targetName = gpTitle;
            }
            else if (review.TargetType == ReviewTargetType.TourGuide && tourGuides.TryGetValue(review.TargetId, out var tgName))
            {
                targetName = tgName;
            }

            dtos.Add(new ReviewReportDto(
                report.Id,
                report.ReviewId,
                report.ReporterId,
                report.Reason.ToString(),
                report.AdditionalNotes,
                report.IsResolved,
                report.CreatedAt,
                authorName,
                review.TargetId,
                targetTypeStr,
                targetName,
                review.Rating,
                review.Comment,
                review.CreatedAt
            ));
        }

        return dtos;
    }

    public async Task<ReviewReport?> GetReportByIdAsync(Guid reportId, CancellationToken ct = default)
    {
        return await context.Set<ReviewReport>()
            .FirstOrDefaultAsync(rr => rr.Id == reportId, ct);
    }
}
