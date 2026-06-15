using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;

namespace Fayora.Application.Features.AdminModule.Commands.CreateMasterAmenity;

public record CreateMasterAmenityCommand(
    string Name,
    AmenityCategory Category,
    string? IconUrl) : ICommand<Result<int>>;