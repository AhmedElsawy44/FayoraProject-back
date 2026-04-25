using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Application.Features.TourGuideModule.Commands.DeactivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;
using Fayora.Contracts.TourGuideModule.CreateTourCompany;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
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

    [HttpPost]
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


    [HttpPatch("tour-guide-package/{PackageId:guid}/activate")]
    public async Task<IActionResult> ActivatePackage(Guid PackageId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateGuidePackageCommand(PackageId), cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPatch("tour-guide-package/{PackageId:guid}/deactivate")]
    public async Task<IActionResult> DeactivatePackage(Guid PackageId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateGuidePackageCommand(PackageId), cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    //[HttpGet("tour-guide-package/{PackageId:guid}")]
    //public async Task<IActionResult> GetPackageById(Guid PackageId, CancellationToken cancellationToken)
    //{
    //    var query = new GetGuidePackageByIdQuery(PackageId);

    //    var result = await sender.Send(query, cancellationToken);

    //    return result.Match(
    //        value => Ok(mapper.Map<GetGuidePackageByIdResponse>(value)),
    //        Problem
    //    );
    //}

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

    //[HttpGet("{id:guid}")]
    //public async Task<IActionResult> GetTourGuideById(Guid id, CancellationToken cancellationToken)
    //{
    //    var query = new GetTourGuideByIdQuery(id);

    //    var result = await sender.Send(query, cancellationToken);

    //    return result.Match(
    //        value => Ok(mapper.Map<GetTourGuideByIdResponse>(value)),
    //        Problem
    //    );
    //}
}
