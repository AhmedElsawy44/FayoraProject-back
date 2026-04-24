using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class TourCompany : GuideAccountBase
{
    public string CompanyName { get; private set; } = null!;
    public GuideStatus Status { get; private set; }
    public bool IsSuperCompany { get; private set; }
    public FileUrl LicenseDocumentUrl { get; private set; } = null!;
    public LicenseClass LicenseClass { get; private set; } = LicenseClass.A;

    private TourCompany(
        Guid userId,
        string companyName,
        string currencyCode = "EGP")
        : base(userId, currencyCode)
    {
        CompanyName = companyName;
        Status = GuideStatus.Pending;
        IsAvailableForBooking = false;
        IsSuperCompany = false;
    }

    private TourCompany()
    {
    }

    public static Result<TourCompany> Create(
        Guid userId,
        string companyName,
        string currencyCode = "EGP")
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return Error.Validation("TourCompany.EmptyName", "Company name cannot be empty.");

        return new TourCompany(userId, companyName, currencyCode);
    }

    public Result<Success> SetLogo(string logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
            return Error.Validation("TourCompany.EmptyLogo", "Logo URL cannot be empty.");

        return Result.Success;
    }

    public Result<Success> UpdateStatus(GuideStatus newStatus)
    {
        if (newStatus == GuideStatus.Pending)
            return Error.Validation("TourCompany.InvalidStatus", "Cannot set status back to Pending.");

        Status = newStatus;
        IsAvailableForBooking = newStatus == GuideStatus.Active;

        return Result.Success;
    }

    public void UpdateRating(float newRating)
    {
        AverageRating = (AverageRating * ReviewCount + newRating) / (ReviewCount + 1);
        ReviewCount++;
    }

    public void IncrementCompletedTours()
    {
        CompletedToursCount++;
    }
}
