using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using Fayora.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourCompanyModule.Commands.UpdateCompanyPackage
{
    public record UpdateCompanyPackageCommand(
        Guid PackageId,
        string Title,
        string Description,
        TourType TourTypes,
        int DurationHours,
        DateOnly StartDate,
        DateOnly EndDate,
        GeoPoint DepartureLocation,
        int MaxCapacity,
        decimal AdultPrice,
        decimal ChildPrice,
        string? CancellationPolicy,
        string? MainImageUrl,
        string? MainVideoUrl,
        string? GuestRequirements,
        List<string> IncludedItems,
        List<string> ExcludedItems
    ) : IRequest<Result<Success>>;
}
