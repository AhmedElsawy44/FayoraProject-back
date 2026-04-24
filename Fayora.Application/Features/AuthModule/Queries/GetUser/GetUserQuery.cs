using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Queries.GetUser;

public record GetUserQuery : IQuery<Result<GetUserResult>>;
