using Fayora.Application.Common.Interfaces.Validations;
using MediatR;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.ResetPasswordPhone;

public record ResetPasswordPhoneCommand(
    string PhoneNumber,
    string ResetToken,
    string NewPassword,
    string DeviceId) : ICommand<Result<Unit>>, ICheckBannedRequest;