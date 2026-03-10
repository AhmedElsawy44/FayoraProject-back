using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Shared.IdentityModule;
using MediatR;

namespace Fayora.Application.Features.Auth.Commands.SendEmailCode;

public record SendEmailCodeCommand(
    string Email,
    string DeviceId,
    CodePurpose Purpose) : IRequest<Result<Unit>>, ICheckBannedRequest;