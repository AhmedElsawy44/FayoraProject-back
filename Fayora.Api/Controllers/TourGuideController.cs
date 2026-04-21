using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.ActivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Application.Features.TourGuideModule.Commands.DeactivateGuidePackage;
using Fayora.Application.Features.TourGuideModule.Commands.DeleteGuidePackage;
using Fayora.Application.Features.TourGuideModule.Queries.GetGuidePackageById;
using Fayora.Application.Features.TourGuideModule.Queries.GetTourGuideById;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.GetGuidePackageById;
using Fayora.Contracts.TourGuideModule.GetTourGuideById;
using Fayora.Domain.Enums.IdentityModule;
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
        var (pricingUnitOk, pricingUnit) = EnumParser.TryParseEnum<PricingUnit>(request.PricingUnit);
        if (!pricingUnitOk)
            return BadRequest("Invalid Pricing Unit");

        var (languageOk, preferredLanguage) = EnumParser.TryParseEnum<Language>(request.PreferredLanguage);
        if (!languageOk)
            return BadRequest("Invalid Preferred Language");

        var (genderOk, gender) = EnumParser.TryParseEnum<Gender>(request.Gender);
        if (!genderOk)
            return BadRequest("Invalid Gender");

        var tourGuideLanguages = new List<UserLanguageProficiencyDto>();
        if (request.TourGuideLanguages is not null && request.TourGuideLanguages.Count != 0)
        {
            foreach (var dto in request.TourGuideLanguages)
            {
                var (langOk, language) = EnumParser.TryParseEnum<Language>(dto.Language);
                if (!langOk)
                    return BadRequest("Invalid Tour Guide Language");

                tourGuideLanguages.Add(new UserLanguageProficiencyDto(language, dto.ProficiencyLevel));
            }
        }

        var command = new CreateTourGuideCommand(
            request.BirthDate,
            gender,
            deviceId,
            request.ProfilePictureUrl,
            request.Description,
            pricingUnit,
            request.BaseRate,
            request.YearsOfExperience,
            request.NationalityCode,
            request.CityIds,
            preferredLanguage,
            tourGuideLanguages,
            request.TimeZone
        );

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<CreateTourGuideResponse>(value)),
            Problem
        );
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

    [HttpGet("tour-guide-package/{PackageId:guid}")]
    public async Task<IActionResult> GetPackageById(Guid PackageId, CancellationToken cancellationToken)
    {
        var query = new GetGuidePackageByIdQuery(PackageId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<GetGuidePackageByIdResponse>(value)),
            Problem
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTourGuideById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTourGuideByIdQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<GetTourGuideByIdResponse>(value)),
            Problem
        );
    }
}
