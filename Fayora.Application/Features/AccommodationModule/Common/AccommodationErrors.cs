using Fayora.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Accommodation.Common;

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

    public static Error RoleNotFound = Error.NotFound(
        "RoleNotFound",
        "The specified role was not found."
    );

    public static Error OwnerProfileAlreadyExists = Error.Validation(
        "OwnerProfileAlreadyExists",
        "An owner profile already exists for this user."
    );
}
