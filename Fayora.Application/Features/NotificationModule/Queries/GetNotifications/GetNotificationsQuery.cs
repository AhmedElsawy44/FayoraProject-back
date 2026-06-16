using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.NotificationModule;
using Fayora.Domain.Common.Results;
using System.Collections.Generic;

namespace Fayora.Application.Features.NotificationModule.Queries.GetNotifications;

public record GetNotificationsQuery(int PageNumber, int PageSize)
    : IQuery<Result<List<InAppNotificationResponse>>>;
