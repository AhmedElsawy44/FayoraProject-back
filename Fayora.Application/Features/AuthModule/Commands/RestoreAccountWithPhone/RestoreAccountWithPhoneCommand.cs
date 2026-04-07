using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone;

public record RestoreAccountWithPhoneCommand(
    string PhoneNumber,
    string Code,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage
) : IRequest<Result<RestoreAccountWithPhoneResult>>, ICheckBannedRequest;
