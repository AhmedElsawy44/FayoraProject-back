using Fayora.Application.Common.Abstractions.Messaging;

namespace Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;

public record GetFinancialStatsQuery(
    DateTime StartDate,
    DateTime EndDate
) : IQuery<FinancialStatsResponse>;
