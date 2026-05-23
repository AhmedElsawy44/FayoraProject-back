using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.NotificationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.NotificationModule.Queries.GetUnreadCount;

public record GetUnreadCountQuery() : IQuery<Result<UnreadCountResponse>>;
