using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeletePhoneAccountCommand;

public record VerifyDeletePhoneAccountCommand(string Code) : IRequest<Result<Success>>;
