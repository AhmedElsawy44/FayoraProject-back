using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Abstractions.Validations;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
namespace Fayora.Application.Features.AuthModule.Commands.SendEmailCode;

public record SendEmailCodeCommand(
    string Email,
    string DeviceId,
    CodePurpose Purpose) : ICommand<Result<Unit>>, ICheckBannedRequest;