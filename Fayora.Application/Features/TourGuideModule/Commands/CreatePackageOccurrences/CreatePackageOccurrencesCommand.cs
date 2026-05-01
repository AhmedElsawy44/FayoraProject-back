using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreatePackageOccurrences
{
    public record CreatePackageOccurrencesCommand(
        Guid PackageId,
        List<OccurrenceItemDto> Occurrences
    ) : ICommand<Result<Success>>;

    public record OccurrenceItemDto(
        DateTime Date,
        int AvailableSeats
    );
}
