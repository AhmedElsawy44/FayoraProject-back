using System;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class PackageMeetingPoint
{
    public Guid Id { get; private set; }
    public Guid PackageId { get; private set; }
    public string MeetingPointName { get; private set; } = null!;
    public GeoPoint MeetingPoint { get; private set; } = null!;
    public TimeOnly Time { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }

    private PackageMeetingPoint() { }

    private PackageMeetingPoint(
        Guid id,
        Guid packageId,
        string meetingPointName,
        GeoPoint meetingPoint,
        TimeOnly time,
        decimal price,
        string? description)
    {
        Id = id;
        PackageId = packageId;
        MeetingPointName = meetingPointName;
        MeetingPoint = meetingPoint;
        Time = time;
        Price = price;
        Description = description;
    }

    public static Result<PackageMeetingPoint> Create(
        Guid packageId,
        string meetingPointName,
        decimal latitude,
        decimal longitude,
        TimeOnly time,
        decimal price,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(meetingPointName))
            return Error.Validation("PackageMeetingPoint.InvalidName", "Meeting point name is required.");

        var meetingPointResult = GeoPoint.Create(latitude, longitude);
        if (meetingPointResult.IsError)
            return meetingPointResult.Errors;

        if (price < 0)
            return Error.Validation("PackageMeetingPoint.InvalidPrice", "Price cannot be negative.");

        return new PackageMeetingPoint(
            Guid.NewGuid(),
            packageId,
            meetingPointName,
            meetingPointResult.Value,
            time,
            price,
            description);
    }

    public void Update(
        string meetingPointName,
        decimal latitude,
        decimal longitude,
        TimeOnly time,
        decimal price,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(meetingPointName))
            throw new ArgumentException("Meeting point name is required.", nameof(meetingPointName));

        var meetingPointResult = GeoPoint.Create(latitude, longitude);
        if (!meetingPointResult.IsError)
        {
            MeetingPoint = meetingPointResult.Value;
        }
        MeetingPointName = meetingPointName;
        Time = time;
        Price = price;
        Description = description;
    }
}
