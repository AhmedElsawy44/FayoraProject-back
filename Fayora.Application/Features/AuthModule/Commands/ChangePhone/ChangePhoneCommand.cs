using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
namespace Fayora.Application.Features.AuthModule.Commands.ChangePhone;

public record ChangePhoneCommand(string PhoneNumber, string Password, string DeviceId, CodeDeliveryMethod DeliveryMethod) : ICheckBannedRequest, ICommand<Result<
    Unit>>;
