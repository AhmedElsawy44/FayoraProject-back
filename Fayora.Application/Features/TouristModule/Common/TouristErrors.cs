using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.Tourist.Common;

public static class TouristErrors
{
    public static Error MasterInterestsNotFound = Error.Validation(
        "MasterInterestsNotFound",
        "one or more master interests were not found."
    );

    public static Error ProfileAlreadyExists = Error.Validation(
        "TouristProfileAlreadyExists",
        "tourist profile already exists for this user."
    );

    public static Error UserAlreadyHasRole = Error.Validation(
        "UserAlreadyHasRole",
        "The user already has the specified role."
    );
}
