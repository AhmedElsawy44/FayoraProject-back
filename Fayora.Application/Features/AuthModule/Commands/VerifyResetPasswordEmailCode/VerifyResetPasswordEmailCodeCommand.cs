using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordEmailCode;

public record VerifyResetPasswordEmailCodeCommand(
string Email,
string DeviceId,
string Code) : ICommand<Result<string>>, ICheckBannedRequest;
