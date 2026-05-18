using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Queries.GetFinancialStats;

public class GetFinancialStatsQueryValidator : AbstractValidator<GetFinancialStatsQuery>
{
    public GetFinancialStatsQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate);
    }
}
