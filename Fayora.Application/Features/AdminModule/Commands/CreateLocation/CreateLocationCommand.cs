using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Commands.CreateLocation
{
    public record CreateLocationCommand(
        string Name,
        string? Description,
        decimal Latitude,
        decimal Longitude,
        string MainImageUrl,
        List<string> ImageUrls) : ICommand<Result<Success>>;
}
