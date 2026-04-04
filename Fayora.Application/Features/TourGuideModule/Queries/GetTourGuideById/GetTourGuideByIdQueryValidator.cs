using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.TourGuideModule.Queries.GetTourGuideById
{
    public class GetTourGuideByIdQueryValidator : AbstractValidator<GetTourGuideByIdQuery>
    {
        public GetTourGuideByIdQueryValidator()
        {
        }
    }
}
