using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Queries.GetVerificationRequest
{
    public record GetVerificationRequestQuery(int RequestId) : IRequest<Result<GetVerificationRequestResponse>>;
}
