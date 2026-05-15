using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.SharedModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.SharedModule;
using Fayora.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Commands.CreateLocation
{

    public class CreateLocationCommandHandler(
        ILocationRepository locationRepository,
        ILocationImageRepository locationImageRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateLocationCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(
            CreateLocationCommand request,
            CancellationToken cancellationToken)
        {
            var mainImageResult = FileUrl.Create(request.MainImageUrl);
            if (mainImageResult.IsError) return mainImageResult.Errors;

            var locationResult = Location.Create(
                request.Name,
                request.Description,
                request.Latitude,
                request.Longitude,
                mainImageResult.Value);
            if (locationResult.IsError) return locationResult.Errors;

            var location = locationResult.Value;

            if (request.ImageUrls?.Any() == true)
            {
                var imageUrlResults = new List<FileUrl>();
                foreach (var url in request.ImageUrls)
                {
                    var imageResult = FileUrl.Create(url);
                    if (imageResult.IsError) return imageResult.Errors;
                    imageUrlResults.Add(imageResult.Value);
                }

                var locationImages = imageUrlResults
                    .Select(imgUrl => new LocationImage(location.Id, imgUrl))
                    .ToList();

                location.AddImages(locationImages.Select(img => img.Id));
                locationImageRepository.AddLocationImages(locationImages);
            }

            locationRepository.AddLocation(location);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
