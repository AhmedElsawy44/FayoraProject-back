using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyDeleteEmailAccount;

public record VerifyDeleteEmailAccountCommand(string Code) : IRequest<Result<Success>>;
