using AutoMapper;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;
using Fayora.Contracts.AccommodationModule.Requests;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Enums.AccommodationModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccommodationController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpPost("profile")]
    public async Task<IActionResult> CreateUnitOwnerProfileAsync(
    [FromBody] CreateUnitOwnerProfileRequest request,
    CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UnitOwnerType>(request.OwnerType, true, out var ownerType))
        {
            return BadRequest("Invalid Unit Owner Type.");
        }

        var command = new CreateUnitOwnerCommand(
            ownerType,
            request.NationalIdUrl,
            request.CommercialName,
            request.TaxRegistrationNumber);

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<CreateUnitOwnerProfileResponse>(value)),
            errors => Problem()
        );
    }
}
