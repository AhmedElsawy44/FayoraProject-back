using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordPhoneCode;

public record VerifyResetPasswordPhoneCodeCommand(
string PhoneNumber,
string DeviceId,
string Code) : IRequest<Result<string>>, ICheckBannedRequest;
