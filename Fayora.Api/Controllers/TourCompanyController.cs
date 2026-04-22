using AutoMapper;
using Fayora.Application.Features.TourCompanyModule.Commands.CreateCompanyPackage;
using Fayora.Application.Features.TourCompanyModule.Commands.DeleteCompanyPackage;
using Fayora.Application.Features.TourCompanyModule.Commands.UpdateCompanyPackage;
using Fayora.Application.Features.TourCompanyModule.CreateTourCompany;
using Fayora.Application.Features.TourCompanyModule.Queries;
using Fayora.Application.Features.TourCompanyModule.Queries.GetAllCompanyPackages;
using Fayora.Application.Features.TourCompanyModule.Queries.GetAllPackages;
using Fayora.Application.Features.TourCompanyModule.Queries.GetCompanyPackages;
using Fayora.Application.Features.TourCompanyModule.Queries.GetTorCompanyById;
using Fayora.Contracts.TourCompanyModule.CreateCompanyPackage;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.GetAllCompanyPackages;
using Fayora.Contracts.TourCompanyModule.GetAllPackages;
using Fayora.Contracts.TourCompanyModule.GetTourCompanyById;
using Fayora.Contracts.TourCompanyModule.UpdateCompanyPackage;
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

        [HttpGet("{companyId:guid}/packages")]
        public async Task<IActionResult> GetCompanyPackages(Guid companyId, CancellationToken ct)
        {
            var query = new GetCompanyPackagesQuery(companyId);
            var result = await sender.Send(query, ct);

            return result.Match(
                onValue: value => Ok(value.Select(p => mapper.Map<GetCompanyPackagesResponse>(p)).ToList()),
                onError: Problem);
        }

        [HttpGet("allPackages")]
        public async Task<IActionResult> GetAllPackages(

           [FromQuery] TourTypeDto? tourType,
           [FromQuery] decimal? minPrice,
           [FromQuery] decimal? maxPrice,
           [FromQuery] DateOnly? startDate,
           [FromQuery] DateOnly? endDate,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           CancellationToken ct = default)
        {
            var query = new GetAllPackagesQuery(
                TourType: tourType.HasValue ? mapper.Map<TourType>(tourType.Value) : null,
                MinPrice: minPrice,
                MaxPrice: maxPrice,
                StartDate: startDate,
                EndDate: endDate,
                Page: page,
                PageSize: pageSize);

            var result = await sender.Send(query, ct);

            return result.Match(
                onValue: value => Ok(mapper.Map<GetAllPackagesResponse>(value)),
                onError: Problem);
        }






        [HttpPut("packages/{id:guid}")]
        public async Task<IActionResult> UpdateCompanyPackage(
             Guid id,
             [FromBody] UpdateCompanyPackageRequest request,
             CancellationToken ct)
        {
            var command = new UpdateCompanyPackageCommand(
                PackageId: id,
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
                onValue: _ => NoContent(),
                onError: Problem);
        }

        [HttpDelete("packages/{id:guid}")]
        public async Task<IActionResult> DeleteCompanyPackage(Guid id, CancellationToken ct)
        {
            var result = await sender.Send(new DeleteCompanyPackageCommand(id), ct);

            return result.Match(
                onValue: _ => NoContent(),
                onError: Problem);
        }



    }
}
