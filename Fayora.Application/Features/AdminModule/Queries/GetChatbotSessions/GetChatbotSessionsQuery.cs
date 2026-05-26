using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.ChatbotMonitoring;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetChatbotSessions;

public record GetChatbotSessionsQuery(
    int PageNumber,
    int PageSize,
    Guid? UserIdFilter,
    DateTime? FromDate,
    DateTime? ToDate) : IQuery<Result<List<GetChatbotSessionsResponse>>>;
