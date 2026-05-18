using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Features.AdminModule.Queries.GetExecutiveDashboard;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;

public record GetFinancialStatsQuery(
    DateTime StartDate,
    DateTime EndDate
) : IQuery<FinancialStatsResponse>;
