using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;

public record LoginWithPhoneCommand(string PhoneNumber, string Password, string DeviceId, string FcmToken, string DeviceLanguage) : ICommand<Result<LoginWithPhoneResult>>, ICheckBannedRequest;
