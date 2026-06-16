using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AdminModule.Commands.Delete;

public record DeleteMasterAmenityCommand(int Id) : ICommand<Result<Unit>>;