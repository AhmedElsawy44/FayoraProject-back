using AutoMapper;
using Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;
using Fayora.Contracts.TourGuideModule.CreateTourCompany;
using Fayora.Domain.Enums.TourGuideModule;
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
    }
}
