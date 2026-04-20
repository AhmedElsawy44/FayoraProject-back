using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.TourGuide;

public class TourGuide : AuditableEntity<Guid>
{
    public Guid UserId { get; init; }
    public decimal BaseRate { get; private set; } //$180/day 
    public PricingUnit? PricingUnit { get; private set; }
    public int YearsOfExperience { get; private set; }
    public string LicenseNumber { get; private set; } = null!;
    public DateOnly LicenseExpiryDate { get; private set; }
    public string TaxRegistrationNumber { get; private set; } = null!;
    public DateOnly? TaxRegistrationDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public int ReviewCount { get; private set; }
    public float AverageRating { get; private set; }
    public GuideStatus Status { get; private set; }
    public bool IsAvailableForBooking { get; private set; }
    public DateTimeOffset? LastActiveDate { get; private set; }
    public bool IsOnline { get; private set; }
    public int CompletedToursCount { get; private set; }
    public bool IsSuperGuide { get; private set; }
    public decimal CancellationRate { get; private set; } // Percentage of tours cancelled by the guide
    public GeoPoint LastLocation { get; private set; } = new GeoPoint(0, 0);
    public DateTimeOffset? LastLocationUpdate { get; private set; }
    public TransportInfo? TransportInfo { get; private set; }
    public float ResponseRate { get; private set; }


    private readonly List<GuideCity> _guideCities = [];
    public IReadOnlyCollection<GuideCity> GuideCities => _guideCities.AsReadOnly();

    private readonly List<GuideTourPackage> _tourPackages = [];
    public IReadOnlyCollection<GuideTourPackage> TourPackages => _tourPackages.AsReadOnly();


    private TourGuide(
        Guid userId,
        PricingUnit pricingUnit,
        decimal baseRate,
        int yearsOfExperience,
        string licenseNumber,
        DateOnly licenseExpiryDate,
        string currencyCode = "EGP")
    {
        UserId = userId;
        PricingUnit = pricingUnit;
        BaseRate = baseRate;
        LicenseNumber = licenseNumber;
        LicenseExpiryDate = licenseExpiryDate;
        CurrencyCode = currencyCode;
        AverageRating = 0f;
        Status = GuideStatus.Pending;
        IsAvailableForBooking = false;
        LastActiveDate = null;
        IsOnline = false;
        CompletedToursCount = 0;
        IsSuperGuide = false;
        CancellationRate = 0;
    }


    private TourGuide()
    {
        CurrencyCode = string.Empty;
    }

    // Factory Methods

    public static Result<TourGuide> Create(Guid userId, PricingUnit pricingUnit, decimal baseRate, int yearsOfExperience, string licenseNumber, DateOnly licenseExpiryDate, string currencyCode = "EGP")
    {
        if (baseRate <= 0)
            return Error.Validation("TourGuide.InvalidBaseRate", "Base rate must be greater than zero.");

        if (licenseExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow))
            return Error.Validation("TourGuide.ExpiredLicense", "License is already expired.");

        return new TourGuide(userId, pricingUnit, baseRate, yearsOfExperience, licenseNumber, licenseExpiryDate, currencyCode);
    }

    public void SetTaxRegistration(string taxRegistrationNumber, DateOnly taxRegistrationDate)
    {
        TaxRegistrationNumber = taxRegistrationNumber;
        TaxRegistrationDate = taxRegistrationDate;
        Updated();
    }


    public void UpdateLocation(decimal latitude, decimal longitude)
    {
        LastLocation = new GeoPoint(latitude, longitude);
        LastLocationUpdate = DateTimeOffset.UtcNow;
        Updated();
    }



    public void SetTransportInfo(bool hasOwnVehicle, string? vehicleDetails, string? transportType)
    {
        TransportInfo = new TransportInfo(hasOwnVehicle, vehicleDetails, transportType);
        Updated();
    }


    public void SetOnlineStatus(bool isOnline)
    {
        IsOnline = isOnline;
        LastActiveDate = DateTimeOffset.UtcNow;
        Updated();
    }


    public Result<Success> SetAvailability(bool isAvailable)
    {
        if (Status != GuideStatus.Active)
            return Error.Validation("TourGuide.NotActive", "Guide must be active to change availability.");
        IsAvailableForBooking = isAvailable;
        Updated();
        return Result.Success;
    }


    public void UpdateRating(float newRating)
    {
        AverageRating = (AverageRating * CompletedToursCount + newRating) / (CompletedToursCount + 1);

        CompletedToursCount++;
        Updated();
    }


    public Result<Success> UpdateStatus(GuideStatus newStatus)
    {
        if (newStatus == GuideStatus.Pending)
            return Error.Validation("TourGuide.InvalidStatus", "Cannot set status back to Pending.");
        Status = newStatus;
        IsAvailableForBooking = newStatus == GuideStatus.Active;
        Updated();
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
        Updated();
    }


    public Result<Success> UpdateBaseRate(decimal newRate, PricingUnit pricingUnit)
    {
        if (newRate <= 0)
            return Error.Validation("TourGuide.InvalidBaseRate", "Base rate must be greater than zero.");
        BaseRate = newRate;
        PricingUnit = pricingUnit;
        Updated();
        return Result.Success;
    }

    public bool IsLicenseValid() => LicenseExpiryDate >= DateOnly.FromDateTime(DateTime.UtcNow);


    public Result<Success> AddCity(int cityId)
    {
        if (_guideCities.Any(c => c.CityId == cityId))
            return Error.Conflict("TourGuide.CityAlreadyAdded", "City already added.");
        _guideCities.Add(new GuideCity(Id, cityId));
        return Result.Success;
    }

    public void RemoveCity(int cityId)
    {
        var city = _guideCities.FirstOrDefault(c => c.CityId == cityId);
        if (city is not null)
            _guideCities.Remove(city);
    }

    public void AddReview(float newRating)
    {
        AverageRating = ((AverageRating * ReviewCount) + newRating) / (ReviewCount + 1);
        ReviewCount++;
        Updated();
    }

    public void MarkTourCompleted()
    {
        CompletedToursCount++;
        Updated();
    }

    public void Verify()
    {
        Status = GuideStatus.Active;
        IsAvailableForBooking = true;
        Updated();
    }
}
