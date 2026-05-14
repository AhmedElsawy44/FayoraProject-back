using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.AdminModule.CreateLocation
{
    public record CreateLocationRequest(
        string Name,
        string? Description,
        decimal Latitude,
        decimal Longitude,
        string MainImageUrl,
       List<string> ImageUrls);
}
