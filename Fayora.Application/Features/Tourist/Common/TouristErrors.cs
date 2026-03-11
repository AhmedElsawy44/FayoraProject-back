using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.Tourist.Common;

public static class TouristErrors
{
    public static Error MasterInterestsNotFound = Error.Validation(
        "MasterInterestsNotFound",
        "one or more master interests were not found."
    );
}
