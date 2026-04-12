using Fayora.Application.Features.TourCompanyModule.CreateTourCompany;
using Fayora.Contracts.TourCompanyModule.CreateTourCompany;
using AutoMapper;
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
    }
}
