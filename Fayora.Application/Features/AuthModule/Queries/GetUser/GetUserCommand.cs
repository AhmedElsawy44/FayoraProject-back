using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public record GetUserCommand : IRequest<Result<GetUserResult>>;
