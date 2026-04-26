using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class TourGuide : GuideAccountBase
{
    public decimal? BaseRate { get; private set; } //$180/day 
    public PricingUnit? PricingUnit { get; private set; }
    public int? YearsOfExperience { get; private set; }
    public string? LicenseNumber { get; private set; } = null;
    public DateOnly? LicenseExpiryDate { get; private set; }
    public ItemStatus Status { get; private set; }
    public bool IsSuperGuide { get; private set; }
    public GeoPoint? LastLocation { get; private set; } = default!;
    public DateTimeOffset? LastLocationUpdate { get; private set; }
    public TransportInfo? TransportInfo { get; private set; }
    public FileUrl? ProfessionalLicenseUrl { get; private set; }

    private readonly List<GuideCity> _guideCities = [];
    public IReadOnlyCollection<GuideCity> GuideCities => _guideCities.AsReadOnly();

    public TourGuide(Guid userId, FileUrl professionalLicenseUrl) : base(userId)
    {
        ProfessionalLicenseUrl = professionalLicenseUrl;
    }


    private TourGuide()
    {
    }


    public void UpdateLocation(GeoPoint lastLocation)
    {
        LastLocation = lastLocation;
        LastLocationUpdate = DateTimeOffset.UtcNow;
    }

    public Result<Success> SetAvailability(bool isAvailable)
    {
        if (Status != ItemStatus.Active)
            return Error.Validation("TourGuide.NotActive", "Guide must be active to change availability.");
        IsAvailableForBooking = isAvailable;
        return Result.Success;
    }


    public void UpdateRating(decimal newRating)
    {
        AverageRating = (AverageRating * CompletedToursCount + newRating) / (CompletedToursCount + 1);

        CompletedToursCount++;
    }


    public Result<Success> UpdateStatus(ItemStatus newStatus)
    {
        if (newStatus == ItemStatus.Pending)
            return Error.Validation("TourGuide.InvalidStatus", "Cannot set status back to Pending.");
        Status = newStatus;
        IsAvailableForBooking = newStatus == ItemStatus.Active;
        return Result.Success;
    }


    public void UpdateCancellationRate(int totalBookings, int cancelledBookings)
    {
        if (totalBookings == 0)
        {
            CancellationRate = 0;
            return;
        }
        CancellationRate = (int)Math.Round((double)cancelledBookings / totalBookings * 100);
    }


    public Result<Success> UpdateBaseRate(decimal newRate, PricingUnit pricingUnit)
    {
        if (newRate <= 0)
            return Error.Validation("TourGuide.InvalidBaseRate", "Base rate must be greater than zero.");
        BaseRate = newRate;
        PricingUnit = pricingUnit;
        return Result.Success;
    }

    public bool IsLicenseValid() => LicenseExpiryDate >= DateOnly.FromDateTime(DateTime.UtcNow);

    public void AddReview(decimal newRating)
    {
        AverageRating = ((AverageRating * ReviewCount) + newRating) / (ReviewCount + 1);
        ReviewCount++;
    }

    public void MarkTourCompleted()
    {
        CompletedToursCount++;
    }

    public void Verify()
    {
        Status = ItemStatus.Active;
        IsAvailableForBooking = true;
    }

    public Result<Success> Update(
        int yearsOfExperience,
        PricingUnit pricingUnit,
        decimal baseRate)
    {
        if (yearsOfExperience < 0)
            return Error.Validation("TourGuide.InvalidExperience", "Years of experience cannot be negative.");
        if (baseRate <= 0)
            return Error.Validation("TourGuide.InvalidBaseRate", "Base rate must be greater than zero.");
        YearsOfExperience = yearsOfExperience;
        PricingUnit = pricingUnit;
        BaseRate = baseRate;
        return Result.Success;
    }

    public void UpdateCities(IEnumerable<GuideCity> newCities)
    {
        var newCityIds = newCities.Select(c => c.CityId).ToList();

        _guideCities.RemoveAll(existingCity => !newCityIds.Contains(existingCity.CityId));

        var existingCityIds = _guideCities.Select(c => c.CityId).ToList();
        var citiesToAdd = newCities.Where(c => !existingCityIds.Contains(c.CityId));

        _guideCities.AddRange(citiesToAdd);
    }
}
