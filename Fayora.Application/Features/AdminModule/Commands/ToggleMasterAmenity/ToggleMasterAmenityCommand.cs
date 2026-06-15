using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;

public record ToggleMasterAmenityCommand(int Id) : ICommand<Result<Unit>>;