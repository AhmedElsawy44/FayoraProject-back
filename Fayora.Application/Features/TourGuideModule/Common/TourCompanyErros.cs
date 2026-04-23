using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.TourGuideModule.Common;

public static class TourCompanyErrors
{
    public static Error TourCompanyIsAlreadyExist => Error.Validation("TourCompany.AlreadyExist", "Tour company is already exist for this user.");
}
