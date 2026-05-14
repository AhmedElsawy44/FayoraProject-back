using Fayora.Domain.Entities.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Persistences.SharedModule
{
    public interface ILocationImageRepository
    {
        void AddLocationImages(List<LocationImage> images);
    }
}
