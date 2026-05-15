using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Commands.CreateLocation
{
    public record CreateLocationCommand(
        string Name,
        string? Description,
        decimal Rating,
        decimal Latitude,
        decimal Longitude,
        LocationCategory Category,
        string MainImageUrl,
        List<string> ImageUrls) : ICommand<Result<int>>;
}
