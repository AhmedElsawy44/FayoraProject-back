using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteEmailAccount;

public record VerifyDeleteEmailAccountCommand(string Code) : ICommand<Result<Success>>;
