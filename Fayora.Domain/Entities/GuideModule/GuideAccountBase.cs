using Fayora.Domain.Common.Interfaces.Admin;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using System.Net.NetworkInformation;

namespace Fayora.Domain.Entities.GuideModule;

public abstract class GuideAccountBase : IVerifiable
{
    public Guid UserId { get; protected set; }
    public string CurrencyCode { get; protected set; } = string.Empty;
    public decimal AverageRating { get; protected set; }
    public int ReviewCount { get; protected set; }
    public int CompletedToursCount { get; protected set; }
    public bool IsAvailableForBooking { get; protected set; }
    public int Views { get; private set; }
    public decimal ResponseRate { get; protected set; }
    public decimal CancellationRate { get; protected set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? AdminNotes { get; private set; }
    public ItemStatus Status { get; private set; }


    public readonly List<Guid> _tourPackageIds = [];
    public IReadOnlyCollection<Guid> TourPackageIds => _tourPackageIds.AsReadOnly();

    protected GuideAccountBase()
    {
        Status = ItemStatus.Pending;
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

    public void IncrementViews() => Views++;

    public Result<Success> Approve()
    {
        if (Status is ItemStatus.Active)
            return Error.Validation("TourGuide.AlreadyActive", "Guide is already active.");

        Status = ItemStatus.Active;
        IsAvailableForBooking = true;
        return Result.Success;
    }

    public Result<Success> Reject(string adminNotes)
    {
        if (Status is ItemStatus.Rejected)
            return Error.Validation("TourGuide.AlreadyRejected", "Guide is already rejected.");
        Status = ItemStatus.Rejected;
        AdminNotes = adminNotes;
        IsAvailableForBooking = false;
        return Result.Success;
    }

}
