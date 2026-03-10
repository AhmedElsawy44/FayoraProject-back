using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Errors;

public static class TouristErrors
{
    public static readonly Error AiQuotaExceeded = Error.Validation(
        "Tourist.AiQuotaExceeded",
        "You have reached your AI message limit for the current plan. Please upgrade your subscription to continue chatting.");
}
