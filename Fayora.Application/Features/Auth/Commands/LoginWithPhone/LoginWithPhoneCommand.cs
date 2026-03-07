using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.LoginWithPhone;

public record LoginWithPhoneCommand(string PhoneNumber, string Password, string DeviceId, string FcmToken, string DeviceLanguage) : IRequest<Result<LoginWithPhoneResult>>, ICheckBannedRequest;
