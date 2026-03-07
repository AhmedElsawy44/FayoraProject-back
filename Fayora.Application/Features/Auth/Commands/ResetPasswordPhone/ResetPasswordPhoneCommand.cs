using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.ResetPasswordPhone;

public record ResetPasswordPhoneCommand(
    string PhoneNumber,
    string ResetToken,
    string NewPassword,
    string DeviceId) : IRequest<Result<Unit>>, ICheckBannedRequest;