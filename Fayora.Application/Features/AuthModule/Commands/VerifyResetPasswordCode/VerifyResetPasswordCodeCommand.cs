using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordCode;

public record VerifyResetPasswordCodeCommand(
string Code,
string Value,
CodeDeliveryMethod Type,
string DeviceId) : ICommand<Result<string>>, ICheckBannedRequest;
