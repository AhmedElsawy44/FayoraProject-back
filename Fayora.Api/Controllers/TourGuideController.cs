using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.CreateGuideWeeklySchedule;
using Fayora.Application.Features.TourGuideModule.Commands.CreatePackageOccurrences;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Application.Features.TourGuideModule.Commands.DeactivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.UpdateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.UpdateTourGuide;
using Fayora.Application.Features.TourGuideModule.Queries.GetMyPackages;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackageAccommodation;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackageDetails;
using Fayora.Application.Features.TourGuideModule.Queries.GetPackagePreview;
using Fayora.Application.Features.TourGuideModule.Queries.GetRecommendedGuides;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Contracts.TourGuideModule.CreateGuidePackage;
using Fayora.Contracts.TourGuideModule.CreatePackageOccurrences;
using Fayora.Contracts.TourGuideModule.CreateTourCompany;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.CreateWeeklySchedule;
using Fayora.Contracts.TourGuideModule.GetMyPackages;
using Fayora.Contracts.TourGuideModule.GetPackageAccommodation;
using Fayora.Contracts.TourGuideModule.GetPackageDetails;
using Fayora.Contracts.TourGuideModule.GetPackagePreview;
using Fayora.Contracts.TourGuideModule.UpdateGuidePackage;
using Fayora.Contracts.TourGuideModule.UpdateTourGuide;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class GuideController(ISender sender, IMapper mapper) : ApiController
{
    [HttpPost("tour-guide/create")]
    public async Task<IActionResult> CreateTourGuide(
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        [FromBody] CreateTourGuideRequest request)
    {

        var command = new CreateTourGuideCommand(
            deviceId,
            request.ProfessionalLicense
        );

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<CreateTourGuideResponse>(value)),
            Problem
        );
    }

    [HttpPost("tour-guide/update")]
    public async Task<IActionResult> UpdateTourGuide(
        [FromBody] UpdateTourGuideRequest request,
        CancellationToken ct)
    {
        var (pricingUnitOk, pricingUnit) = EnumParser.TryParseEnum<PricingUnit>(request.PricingUnit);
        if (!pricingUnitOk)
            return BadRequest("Invalid Pricing Unit");

        var command = new UpdateTourGuideCommand(
            request.YearsOfExperience,
            pricingUnit,
            request.BaseRate,
            request.CoveredCities
        );

        var result = await sender.Send(command, ct);

        return result.Match(value => Ok(value), Problem);
    }

    [HttpPost("tour-company/create")]
    public async Task<IActionResult> CreateTourCompany(
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        [FromBody] CreateTourCompanyRequest request,
        CancellationToken ct)
    {
        var (licenseClassOk, licenseClass) = EnumParser.TryParseEnum<LicenseClass>(request.LicenseClass);
        if (!licenseClassOk)
            return BadRequest("Invalid License Class");

        var command = new CreateTourCompanyCommand(
            deviceId,
            request.CompanyName,
            request.ProfilePictureUrl,
            request.LicenseDocumentUrl,
            licenseClass
            );

        var result = await sender.Send(command, ct);

        return result.Match(
            onValue: value => Ok(mapper.Map<CreateTourCompanyResponse>(value)),
            onError: Problem);
    }

    [HttpPost("package/create")]
    public async Task<IActionResult> CreateGuidePackage(
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        [FromBody] CreateGuidePackageRequest request,
        CancellationToken ct)
    {
        var (tourTypeOk, tourType) = EnumParser.TryParseEnum<TourType>(request.TourType);
        if (!tourTypeOk)
            return BadRequest("Invalid Tour Type");

        var (transportTypeOk, transportType) = EnumParser.TryParseEnum<TransportType>(request.TransportType);
        if (!transportTypeOk)
            return BadRequest("Invalid Transport Type");

        var (cancellationPolicyOk, cancellationPolicy) = EnumParser.TryParseEnum<CancellationPolicy>(request.CancellationPolicy);
        if (!cancellationPolicyOk)
            return BadRequest("Invalid Cancellation Policy");

        var command = new CreateGuidePackageCommand(
            request.Title,
            request.Description,
            tourType,
            request.DurationHours,
            request.NumOfDays,
            request.Nights,
            request.Longitude,
            request.Latitude,
            transportType,
            request.ArrivalNote,
            request.AdultPrice,
            request.ChildPrice,
            request.MaxCapacity,
            request.IncludedIds,
            request.ExcludedIds,
            request.MainImageUrl,
            request.VideoURL,
            request.ImageURLs,
            request.GuestRequirements,
            cancellationPolicy,
            request.Activities,
            request.LocationIds,
            request.OptionalActivities
            );

        var result = await sender.Send(command, ct);

        return result.Match(Ok, Problem);
    }


    [HttpPut("package/{packageId:guid}")]
    public async Task<IActionResult> UpdateGuidePackage(
        [FromRoute] Guid packageId,
        [FromBody] UpdateGuidePackageRequest request,
        CancellationToken ct)
    {
        var (tourTypeOk, tourType) = EnumParser.TryParseEnum<TourType>(request.TourType);
        if (!tourTypeOk)
            return BadRequest("Invalid Tour Type");

        var (transportTypeOk, transportType) = EnumParser.TryParseEnum<TransportType>(request.TransportType);
        if (!transportTypeOk)
            return BadRequest("Invalid Transport Type");

        var (cancellationPolicyOk, cancellationPolicy) = EnumParser.TryParseEnum<CancellationPolicy>(request.CancellationPolicy);
        if (!cancellationPolicyOk)
            return BadRequest("Invalid Cancellation Policy");

        var command = new UpdateGuidePackageCommand(
            packageId,
            request.Title,
            request.Description,
            tourType,
            request.DurationHours,
            request.NumOfDays,
            request.Nights,
            request.Longitude,
            request.Latitude,
            transportType,
            request.ArrivalNote,
            request.AdultPrice,
            request.ChildPrice,
            request.MaxCapacity,
            request.IncludedIds,
            request.ExcludedIds,
            request.MainImageUrl,
            request.VideoURL,
            request.ImageURLs,
            request.GuestRequirements,
            cancellationPolicy,
            request.Activities,
            request.LocationIds,
            request.OptionalActivities
        );

        var result = await sender.Send(command, ct);

        return result.Match(value => Ok(new { packageId = value }), Problem);
    }

    [HttpPatch("package/{PackageId:guid}/activate")]
    public async Task<IActionResult> ActivatePackage(Guid PackageId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateGuidePackageCommand(PackageId), cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPatch("package/{PackageId:guid}/deactivate")]
    public async Task<IActionResult> DeactivatePackage(Guid PackageId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateGuidePackageCommand(PackageId), cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("package/{PackageId:guid}")]
    public async Task<IActionResult> DeletePackage(Guid PackageId, CancellationToken cancellationToken)
    {
        var command = new DeleteGuidePackageCommand(PackageId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }


    [HttpPost("{packageId:guid}/occurrences")]
    public async Task<IActionResult> CreateOccurrences(
    Guid packageId,
    CreatePackageOccurrencesRequest request,
    CancellationToken cancellationToken)
    {
        var command = new CreatePackageOccurrencesCommand(
            packageId,
            [.. request.Occurrences.Select(x => new OccurrenceItemDto(x.Date, x.AvailableSeats))]
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
            );
    }

    [HttpPost("schedules")]
    public async Task<IActionResult> CreateWeeklySchedule(
        [FromBody] CreateWeeklyScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateGuideWeeklyScheduleCommand(
            request.DayOfWeek,
            request.StartTime,
            request.EndTime
        );

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    //for tour guides to view package beforw publishing it to tourists

    [HttpGet("packages/{packageId:guid}/preview")]
    public async Task<IActionResult> GetPackagePreview(
    [FromRoute] Guid packageId,
    CancellationToken cancellationToken)
    {
        var query = new GetPackagePreviewQuery(packageId);
        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(mapper.Map<PackagePreviewResponse>(value)),
            Problem);
    }


    //for tourists to view package details before booking
    [HttpGet("packages/{packageId:guid}")]
    public async Task<IActionResult> GetPackageDetails(
    [FromRoute] Guid packageId,
    CancellationToken cancellationToken)
    {
        var query = new GetPackageDetailsQuery(packageId);
        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(mapper.Map<PackageDetailsResponse>(value)),
            Problem);
    }



    [HttpGet("my-packages")]
    public async Task<IActionResult> GetMyPackages(
    [FromQuery] string? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        ItemStatus? parsedStatus = null;
        if (!string.IsNullOrEmpty(status))
        {
            if (!Enum.TryParse<ItemStatus>(status, true, out var s))
                return BadRequest("Invalid status value.");
            parsedStatus = s;
        }

        var query = new GetMyPackagesQuery(parsedStatus, page, pageSize);
        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<MyPackagesResponse>(value)),
            Problem);
    }

    /// <summary>
    /// Returns personalized package recommendations for authenticated users,
    /// or trending packages for anonymous users.
    /// Uses a multi-signal scoring engine (content-based, collaborative, popularity, recency).
    /// </summary>
    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommendedPackages(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRecommendedPackagesQuery(count);
        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(value),
            Problem);
    }

    /// <summary>
    /// Returns personalized tour guide recommendations for authenticated users,
    /// or trending guides for anonymous users.
    /// Uses a multi-signal scoring engine (content-based, collaborative, popularity, recency).
    /// </summary>
    [HttpGet("recommended-guides")]
    public async Task<IActionResult> GetRecommendedGuides(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRecommendedGuidesQuery(count);
        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(value),
            Problem);
    }


    [HttpGet("package-accommodations/{packageAccommodationId:guid}")]
    public async Task<IActionResult> GetPackageAccommodation(
    [FromRoute] Guid packageAccommodationId,
    CancellationToken cancellationToken)
    {
        var query = new GetPackageAccommodationQuery(packageAccommodationId);
        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(mapper.Map<GetPackageAccommodationResponse>(value)),
            Problem);
    }
}
