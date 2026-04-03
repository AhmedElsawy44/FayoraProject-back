using Fayora.Domain.Entities.TourGuide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;

public interface ITourGuidePackageRepository
{
    Task AddPackageAsync(GuideTourPackage package, CancellationToken cancellationToken);
}
