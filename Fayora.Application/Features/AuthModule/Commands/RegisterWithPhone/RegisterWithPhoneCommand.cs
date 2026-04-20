using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.RegisterWithPhone;

public record RegisterWithPhoneCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Password,
    CodeDeliveryMethod DeliveryMethod,
    string DeviceId) : ICommand<Result<RegisterWithPhoneResult>>, ICheckBannedRequest;