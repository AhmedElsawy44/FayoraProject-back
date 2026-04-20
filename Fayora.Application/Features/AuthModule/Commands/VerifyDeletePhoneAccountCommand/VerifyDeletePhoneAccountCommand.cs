using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeletePhoneAccountCommand;

public record VerifyDeletePhoneAccountCommand(string Code) : ICommand<Result<Success>>;
