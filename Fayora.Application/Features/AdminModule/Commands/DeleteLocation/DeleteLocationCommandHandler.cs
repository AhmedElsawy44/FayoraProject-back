using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteLocation
{
    public class DeleteLocationCommandHandler(
        ILocationRepository locationRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<DeleteLocationCommand, Result<string>>
    {

        public async Task<Result<string>> Handle(
    DeleteLocationCommand request,
    CancellationToken cancellationToken)
        {
            var location = await locationRepository.GetLocationByIdAsync(
                request.LocationId,
                cancellationToken);

            if (location is null)
                return Error.NotFound("Location.NotFound", "Location not found.");

            locationRepository.RemoveLocation(location);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return "Location deleted successfully.";
        }

    }
}
