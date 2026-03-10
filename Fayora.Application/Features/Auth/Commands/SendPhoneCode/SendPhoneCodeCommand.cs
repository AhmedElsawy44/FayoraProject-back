using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.SendPhoneCode;

public record SendPhoneCodeCommand(
    string PhoneNumber,
    string DeviceId,
    CodePurpose Purpose,
    CodeDeliveryMethod DeliveryMethod) : IRequest<Result<Unit>>, ICheckBannedRequest;