using AutoMapper;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnit;
using Fayora.Application.Features.AccommodationModule.Commands.CreateUnitOwner;
using Fayora.Contracts.AccommodationModule.Requests;
using Fayora.Contracts.AccommodationModule.Responses;
using Fayora.Domain.Enums.AccommodationModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccommodationController(ISender sender, IMapper mapper) : ApiController
{
    [HttpPost("owner-profile")]
    public async Task<IActionResult> CreateUnitOwnerProfileAsync(
    [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
    [FromBody] CreateUnitOwnerProfileRequest request,
    CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UnitOwnerType>(request.OwnerType, true, out var ownerType))
        {
            return BadRequest("Invalid Unit Owner Type.");
        }

        var command = new CreateUnitOwnerCommand(
            deviceId,
            ownerType,
            request.CommercialName);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<CreateUnitOwnerProfileResponse>(value)),
            errors => Problem()
        );
    }

    [HttpPost("housing-units")]
    public async Task<IActionResult> CreateUnit(
    [FromBody] CreateUnitRequest request,
    CancellationToken cancellationToken)
    {
        var amenities = new HashSet<Amenities>();

        foreach (var amenity in request.Amenities)
        {
            var (amenityOk, amenityValue) = EnumParser.TryParseEnum<Amenities>(amenity);
            if (amenityOk)
            {
                amenities.Add(amenityValue);
            }
        }
        var command = new CreateUnitCommand(
            request.Title,
            request.Description,
            request.LocationId,
            request.AddressDetails,
            request.Latitude,
            request.Longitude,
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
            amenities
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(value),
            errors => Problem()
        );
    }

    //[HttpGet("housing-units/{id:guid}")]
    //public async Task<IActionResult> GetUnitById(
    //    [FromRoute] Guid id,
    //    CancellationToken cancellationToken)
    //{
    //    var query = new GetUnitByIdQuery(id);

    //    var result = await sender.Send(query, cancellationToken);

    //    return result.Match(
    //        value => Ok(value),
    //        errors => Problem()
    //    );
    //}

    //[HttpPost("housing-units/{id:guid}/views")]
    //public async Task<IActionResult> IncrementUnitViews(
    //    [FromRoute] Guid id,
    //    CancellationToken cancellationToken)
    //{
    //    var command = new IncrementUnitViewsCommand(id);

    //    var result = await sender.Send(command, cancellationToken);

    //    return result.Match(
    //        _ => NoContent(), // 204 No Content لأن مفيش داتا هترجع للموبايل
    //        errors => Problem()
    //    );
    //}
}