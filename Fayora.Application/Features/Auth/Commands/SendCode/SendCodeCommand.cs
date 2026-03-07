using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.SendCode;

public record SendCodeCommand(string? Email, string? PhoneNumber, string DeviceId, CodePurpose OtpPurpose, CodeDeliveryMethod DeliveryMethod) : IRequest<Result<Unit>>, ICheckBannedRequest
{
    public string Identity => Email ?? PhoneNumber ?? "";
    public bool IsEmail => Email is not null;
}
