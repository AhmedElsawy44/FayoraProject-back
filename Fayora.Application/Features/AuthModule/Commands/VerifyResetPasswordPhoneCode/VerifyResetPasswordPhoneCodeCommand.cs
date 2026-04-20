using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordPhoneCode;

public record VerifyResetPasswordPhoneCodeCommand(
string PhoneNumber,
string DeviceId,
string Code) : ICommand<Result<string>>, ICheckBannedRequest;
