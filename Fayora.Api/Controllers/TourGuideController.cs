using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;
using Fayora.Contracts.TourGuideModule.CreateTourGuide;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class TourGuideController(ISender sender, IMapper mapper) : ApiController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateTourGuide(
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        [FromBody] CreateTourGuideRequest request)
    {
        var command = new CreateTourGuideCommand(
            deviceId,
            request.ProfilePictureUrl,
            request.Description,
            request.PricingUnit,
            request.BaseRate,
            request.YearsOfExperience,
            request.LicenseNumber,
            request.LicenseExpiryDate,
            request.CurrencyCode,
            request.CityIds,
            request.PreferredLanguage,
            request.TourGuideLanguages
        );

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<CreateTourGuideResponse>(value)),
            Problem
        );
    }
}
