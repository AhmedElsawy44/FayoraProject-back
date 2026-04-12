using AutoMapper;
using Fayora.Application.Features.TourCompanyModule.CreateTourCompany;
using Fayora.Application.Features.TourCompanyModule.Commands.CreateCompanyPackage;
using Fayora.Application.Features.TourCompanyModule.Queries;
using Fayora.Contracts.TourCompanyModule.CreateCompanyPackage;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.GetTourCompanyById;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers
{

    [Route("api/companies")]
    public class TourCompanyController(ISender sender, IMapper mapper) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateTourCompany(
            [FromBody] CreateTourCompanyRequest request,
            CancellationToken ct)
        {
            var command = new CreateTourCompanyCommand(
                UserId: request.UserId,
                CompanyName: request.CompanyName,
                Description: request.Description,
                CommercialRegisterNumber: request.CommercialRegisterNumber,
                TaxRegistrationNumber: request.TaxRegistrationNumber,
                CurrencyCode: request.CurrencyCode,
                LogoUrl: request.LogoUrl);

            var result = await sender.Send(command, ct);

            return result.Match(
                onValue: value => Ok(mapper.Map<CreateTourCompanyResponse>(value)),
                onError: Problem);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTourCompany(Guid id, CancellationToken ct)
        {
            var query = new GetTourCompanyByIdQuery(id);
            var result = await sender.Send(query, ct);

            return result.Match(
                onValue: value => Ok(mapper.Map<GetTourCompanyByIdResponse>(value)),
                onError: Problem);
        }



        [HttpPost("packages")]
        public async Task<IActionResult> CreateCompanyPackage(
    [FromBody] CreateCompanyPackageRequest request,
    CancellationToken ct)
        {
            var command = new CreateCompanyPackageCommand(
                CompanyId: request.CompanyId,
                Title: request.Title,
                Description: request.Description,
                TourTypes: mapper.Map<TourType>(request.TourTypes),
                DurationHours: request.DurationHours,
                StartDate: request.StartDate,
                EndDate: request.EndDate,
                DepartureLocation: new GeoPoint(request.DepartureLat, request.DepartureLng),
                MaxCapacity: request.MaxCapacity,
                AdultPrice: request.AdultPrice,
                ChildPrice: request.ChildPrice,
                CancellationPolicy: request.CancellationPolicy,
                MainImageUrl: request.MainImageUrl,
                MainVideoUrl: request.MainVideoUrl,
                GuestRequirements: request.GuestRequirements,
                IncludedItems: request.IncludedItems,
                ExcludedItems: request.ExcludedItems);

            var result = await sender.Send(command, ct);

            return result.Match(
                onValue: value => Ok(mapper.Map<CreateCompanyPackageResponse>(value)),
                onError: Problem);
        }
    }
}
