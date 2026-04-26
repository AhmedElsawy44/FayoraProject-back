using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.CreateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Application.Features.TourGuideModule.Commands.DeactivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.UpdateTourGuide;
using Fayora.Contracts.TourGuideModule.CreateGuidePackage;
using Fayora.Contracts.TourGuideModule.CreateTourCompany;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.UpdateTourGuide;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class TourGuideController(ISender sender, IMapper mapper) : ApiController
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
            request.Activities
            );

        var result = await sender.Send(command, ct);

        return result.Match(Ok, Problem);
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

    [HttpDelete("tour-guide-package/{PackageId:guid}")]
    public async Task<IActionResult> DeletePackage(Guid PackageId, CancellationToken cancellationToken)
    {
        var command = new DeleteGuidePackageCommand(PackageId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
