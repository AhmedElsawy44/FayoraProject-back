using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Application.Features.TourGuideModule.Commands.UpdateCompanyPackage;
using Fayora.Contracts.TourCompanyModule.CreateCompanyPackage;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.GetAllCompanyPackages;
using Fayora.Contracts.TourCompanyModule.GetAllPackages;
using Fayora.Contracts.TourCompanyModule.GetTourCompanyById;
using Fayora.Contracts.TourCompanyModule.UpdateCompanyPackage;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.Enums.TourGuideModule;
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
                request.Description,
                request.ProfilePictureUrl,
                request.LicenseDocumentUrl,
                licenseClass
                );

            var result = await sender.Send(command, ct);

            return result.Match(
                onValue: value => Ok(mapper.Map<CreateTourCompanyResponse>(value)),
                onError: Problem);
        }

        //[HttpGet("{id:guid}")]
        //public async Task<IActionResult> GetTourCompany(Guid id, CancellationToken ct)
        //{
        //    var query = new GetTourCompanyByIdQuery(id);
        //    var result = await sender.Send(query, ct);

        //    return result.Match(
        //        onValue: value => Ok(mapper.Map<GetTourCompanyByIdResponse>(value)),
        //        onError: Problem);
        //}

        //[HttpGet("{companyId:guid}/packages")]
        //public async Task<IActionResult> GetCompanyPackages(Guid companyId, CancellationToken ct)
        //{
        //    var query = new GetCompanyPackagesQuery(companyId);
        //    var result = await sender.Send(query, ct);

        //    return result.Match(
        //        onValue: value => Ok(value.Select(p => mapper.Map<GetCompanyPackagesResponse>(p)).ToList()),
        //        onError: Problem);
        //}

        //[HttpGet("allPackages")]
        //public async Task<IActionResult> GetAllPackages(

        //   [FromQuery] TourTypeDto? tourType,
        //   [FromQuery] decimal? minPrice,
        //   [FromQuery] decimal? maxPrice,
        //   [FromQuery] DateOnly? startDate,
        //   [FromQuery] DateOnly? endDate,
        //   [FromQuery] int page = 1,
        //   [FromQuery] int pageSize = 10,
        //   CancellationToken ct = default)
        //{
        //    var query = new GetAllPackagesQuery(
        //        TourType: tourType.HasValue ? mapper.Map<TourType>(tourType.Value) : null,
        //        MinPrice: minPrice,
        //        MaxPrice: maxPrice,
        //        StartDate: startDate,
        //        EndDate: endDate,
        //        Page: page,
        //        PageSize: pageSize);

        //    var result = await sender.Send(query, ct);

        //    return result.Match(
        //        onValue: value => Ok(mapper.Map<GetAllPackagesResponse>(value)),
        //        onError: Problem);
        //}






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
                DepartureLocation: GeoPoint.Create(request.DepartureLat, request.DepartureLng).Value,
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
    }
}
