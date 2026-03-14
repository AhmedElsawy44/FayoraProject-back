using AutoMapper;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnit;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;
using Fayora.Contracts.AccommodationModule.Requests;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccommodationController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpPost("owner-profile")]
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

    [HttpPost("housing-units")]
    public async Task<IActionResult> CreateUnit(
    [FromBody] CreateUnitRequest request)
    {
        var command = new CreateUnitCommand(
            request.Title,
            request.Description,
            request.LocationId,
            request.AddressDetails,
            new GeoPoint(request.Latitude, request.Longitude), 
            Enum.Parse<HousingType>(request.Type, true),
            request.PricePerNight,
            request.NumberOfRooms,
            request.BedRooms,
            request.BathRooms,
            request.NumberOfBeds,
            request.MaxGuests,
            request.CheckInTime,
            request.CheckOutTime,
            request.MainImageUrl,
            request.VerificationRequestId,
            request.ImageUrls,
            request.AmenityIds
        );

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(value),
            errors => Problem()
        );
    }
}
