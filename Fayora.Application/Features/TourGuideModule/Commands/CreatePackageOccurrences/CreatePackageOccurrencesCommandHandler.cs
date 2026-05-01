using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreatePackageOccurrences
{
    public class CreatePackageOccurrencesCommandHandler(
        IPackageOccurrenceRepository repository,
        IClientContextProvider clientContextProvider
    ) : ICommandHandler<CreatePackageOccurrencesCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            CreatePackageOccurrencesCommand request,
            CancellationToken cancellationToken)
        {
            var currentUserId = clientContextProvider.GetContext().UserId;

            var packageExists = await repository.PackageExistsForUserAsync(
                 request.PackageId, currentUserId, cancellationToken);

            if (!packageExists)
                return Error.NotFound("Package not found.");

            var dates = request.Occurrences.Select(x => x.Date).ToList();

            var hasOverlap = await repository.HasOverlappingOccurrenceAsync(
                request.PackageId, dates, cancellationToken);

            if (hasOverlap)
                return Error.Conflict("One or more dates already have an occurrence.");

            var occurrences = request.Occurrences
                .Select(x => new PackageOccurrence(request.PackageId, x.Date, x.AvailableSeats))
                .ToList();

            await repository.AddRangeAsync(occurrences, cancellationToken);

            return Result.Success;
        }
    }
}
