using Fayora.Domain.Common.Results;

namespace Fayora.Domain.Entities.GuideModule;

public abstract class GuideAccountBase
{
    public Guid UserId { get; protected set; }
    public string CurrencyCode { get; protected set; } = string.Empty;
    public float AverageRating { get; protected set; }
    public int ReviewCount { get; protected set; }
    public int CompletedToursCount { get; protected set; }
    public bool IsAvailableForBooking { get; protected set; }
    public float ResponseRate { get; protected set; }
    public decimal CancellationRate { get; protected set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public readonly List<Guid> _tourPackageIds = [];
    public IReadOnlyCollection<Guid> TourPackageIds => _tourPackageIds.AsReadOnly();

    protected GuideAccountBase()
    {
    }

    protected GuideAccountBase(Guid userId, string currencyCode = "EGP")
    {
        UserId = userId;
        CurrencyCode = currencyCode;
    }

    public Result<Success> DeletePackage(Guid packageId)
    {
        if (_tourPackageIds.Contains(packageId))
        {
            _tourPackageIds.Remove(packageId);
            return Result.Success;
        }
        return Error.NotFound(
        "TourGuide.PackageNotFound",
        "The package you are trying to delete was not found in this guide's list.");
    }
}
