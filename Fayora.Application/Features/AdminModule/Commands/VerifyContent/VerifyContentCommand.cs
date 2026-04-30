using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.Admin.Commands.VerifyContent;

public record VerifyContentCommand(
    Guid EntityId,
    string EntityType,
    bool IsApproved,
    string AdminNotes) : ICommand<Result<Success>>;
