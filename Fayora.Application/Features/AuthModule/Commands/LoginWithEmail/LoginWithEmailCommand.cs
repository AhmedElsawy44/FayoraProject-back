using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public record LoginWithEmailCommand(
    string Email,
    string Password,
    string DeviceId,
    string FcmToken,
    string DeviceLanguage) : IRequest<Result<LoginWithEmailResult>>, ICheckBannedRequest;
