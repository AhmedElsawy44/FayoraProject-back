using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteAccommodation;

public record DeleteAccommodationCommand(Guid Id) : ICommand<Result<Success>>;
