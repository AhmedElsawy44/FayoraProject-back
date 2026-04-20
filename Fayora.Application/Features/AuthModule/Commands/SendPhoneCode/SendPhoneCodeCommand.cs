using Fayora.Application.Common.Interfaces.Validations;
using MediatR;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.SendPhoneCode;

public record SendPhoneCodeCommand(
    string PhoneNumber,
    string DeviceId,
    CodePurpose Purpose,
    CodeDeliveryMethod DeliveryMethod) : ICommand<Result<Unit>>, ICheckBannedRequest;