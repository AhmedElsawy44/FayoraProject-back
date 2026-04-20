using Fayora.Domain.Common.Results;
using Fayora.Application.Abstractions.Messaging;
namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public record GetUserCommand : IQuery<Result<GetUserResult>>;
