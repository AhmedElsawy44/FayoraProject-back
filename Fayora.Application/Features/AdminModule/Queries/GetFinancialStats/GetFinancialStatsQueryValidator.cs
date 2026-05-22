using FluentValidation;

namespace Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;

public class GetFinancialStatsQueryValidator : AbstractValidator<GetFinancialStatsQuery>
{
    public GetFinancialStatsQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate);
    }
}
