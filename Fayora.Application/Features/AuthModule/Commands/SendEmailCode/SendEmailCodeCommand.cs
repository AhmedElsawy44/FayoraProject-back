using Fayora.Application.Common.Interfaces.Validations;
using MediatR;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
namespace Fayora.Application.Features.AuthModule.Commands.SendEmailCode;

public record SendEmailCodeCommand(
    string Email,
    string DeviceId,
    CodePurpose Purpose) : ICommand<Result<Unit>>, ICheckBannedRequest;