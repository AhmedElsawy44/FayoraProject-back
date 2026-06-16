using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteGuide;

public record DeleteGuideCommand(Guid Id) : ICommand<Result<Success>>;
