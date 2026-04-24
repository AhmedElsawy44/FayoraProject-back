using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteAccount;

public record VerifyDeleteAccountCommand(string Code, CodeDeliveryMethod Type) : ICommand<Result<Success>>;