using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.RegisterWithPhone;

public record RegisterWithPhoneCommand(
    string PhoneNumber,
    string Password,
    CodeDeliveryMethod DeliveryMethod,
    string DeviceId) : IRequest<Result<RegisterWithPhoneResult>>, ICheckBannedRequest;