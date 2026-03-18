using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AccommodationModule.Common;

public static class AccommodationErrors
{
    public static Error OwnerIsAleadyExist = Error.Validation(
        "OwnerIsAlreadyExist",
        "The user is already an owner."
    );

    public static Error UserNotFound = Error.NotFound(
        "UserNotFound",
        "The user was not found."
    );

    public static Error UserAlreadyHasRole = Error.Validation(
        "UserAlreadyHasRole",
        "The user already has the specified role."
    );

    public static Error OwnerProfileAlreadyExists = Error.Validation(
        "OwnerProfileAlreadyExists",
        "An owner profile already exists for this user."
    );

    public static Error OwnerProfileNotFound = Error.NotFound(
            "OwnerProfileNotFound",
            "The owner profile was not found."
    );

    public static Error UnitNotFound = Error.NotFound(
        "UnitNotFound",
        "The housing unit was not found."
    );

    public static Error UnitAlreadyExists = Error.Validation(
       "UnitAlreadyExists",
       "A housing unit with the same details already exists."
   );

    public static Error RoleNotFound = Error.NotFound(
        "RoleNotFound",
        "The specified role was not found."
    );
}
