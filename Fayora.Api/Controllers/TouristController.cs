using AutoMapper;
using Fayora.Application.Features.TouristModule.Commands.CreateTouristProfile;
using Fayora.Application.Features.TouristModule.Commands.TrackUserInteraction;
using Fayora.Application.Features.TouristModule.Queries.GetAllActivePackages;
using Fayora.Application.Features.TouristModule.Queries.GetAllLocations;
using Fayora.Application.Features.TouristModule.Queries.GetInterests;
using Fayora.Application.Features.TouristModule.Queries.GetLocationDetails;
using Fayora.Application.Features.TouristModule.Queries.GetRecommendedPackages;
using Fayora.Contracts.AdminModule.CreateLocation;
using Fayora.Contracts.TouristModule;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.Enums.TouristModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TouristController(ISender sender, IMapper mapper) : ApiController
{
    [HttpGet("Interests")]
    public async Task<IActionResult> GetInterestsAsync(CancellationToken cancellationToken)
    {
        var query = new GetInterestsQuery();

        var result = await sender.Send(query, cancellationToken);

        return Ok(mapper.Map<InterestsResponse>(result));
    }

    [HttpPost("profile")]
    public async Task<IActionResult> CreateTouristProfileAsync(
    [FromBody] CreateTouristRequest request,
    [FromHeader(Name = "X-Device-Id")] string deviceId,
    CancellationToken cancellationToken)
    {
        BudgetTier? budgetTier = null;
        TravelStyle? travelStyle = null;

        if (!string.IsNullOrWhiteSpace(request.BudgetTier))
        {
            if (!Enum.TryParse<BudgetTier>(request.BudgetTier, true, out var parsedBudget))
                return BadRequest("Invalid budget tier.");
            budgetTier = parsedBudget;
        }

        if (!string.IsNullOrWhiteSpace(request.TravelStyle))
        {
            if (!Enum.TryParse<TravelStyle>(request.TravelStyle, true, out var parsedStyle))
                return BadRequest("Invalid travel style.");
            travelStyle = parsedStyle;
        }

        var command = new CreateTouristProfileCommand(
            deviceId,
            budgetTier,
            travelStyle,
            request.InterestIds);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<CreateTouristResponse>(value)),
            Problem
        );
    }

    [HttpPost("track")]
    public async Task<IActionResult> Track(
        [FromBody] TrackUserInteractionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new TrackUserInteractionCommand(
            request.EntityId,
            (EntityType)request.EntityType,
            (InteractionType)request.InteractionType
        );

        var result = await sender.Send(command, cancellationToken);
        return result.Match(
             _ => NoContent(),
           errors => Problem(errors)
 );
    }


    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations(
        [FromQuery] LocationCategoryDto? category,
        [FromQuery] decimal? minRating,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        LocationCategory? domainCategory = null;
        if (category.HasValue)
            domainCategory = mapper.Map<LocationCategory>(category.Value);

        var query = new GetAllLocationsQuery(
            domainCategory,
            minRating,
            search,
            page,
            pageSize);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<GetAllLocationsResponse>(value)),
            Problem);
    }

    [HttpGet("ActivePackages")]
    public async Task<IActionResult> GetActivePackages(
    [FromQuery] string? search,
    [FromQuery] int? locationId,
    [FromQuery] ProviderType? providerType,
    [FromQuery] TourType? tourType,
    [FromQuery] int? minDuration,
    [FromQuery] int? maxDuration,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {

        var query = new GetActivePackagesQuery(
            search,
            locationId,
            providerType,
            tourType,
            minDuration,
            maxDuration,
            minPrice,
            maxPrice,
            page,
            pageSize);

        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(mapper.Map<ActivePackagesResponse>(value)),
            Problem);
    }

    [HttpGet("{locationId:int}")]
    public async Task<IActionResult> GetLocationDetails(
    [FromRoute] int locationId,
    CancellationToken cancellationToken)
    {
        var query = new GetLocationDetailsQuery(locationId);
        var result = await sender.Send(query, cancellationToken);
        return result.Match(
            value => Ok(mapper.Map<LocationDetailsResponse>(value)),
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
}

