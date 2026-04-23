using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;

public record CreateTourCompanyCommand(
    string DeviceId,
    string CompanyName,
    string Description,
    string ProfilePictureUrl,
    string LicenseDocumentUrl,
    LicenseClass LicenseClass

) : ICommand<Result<CreateTourCompanyResult>>;
