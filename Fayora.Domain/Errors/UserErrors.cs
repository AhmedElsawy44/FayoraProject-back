using Fayora.Domain.Common.Results;

namespace Fayora.Domain.Errors;

public static class UserErrors
{
    public static readonly Error InvalidEmail = Error.Validation(
        "User.InvalidEmail",
        "The email address is invalid."
    );

    public static readonly Error InvalidPhone = Error.Validation(
        "User.InvalidPhone",
        "The phone number is invalid."
    );

    public static readonly Error EmailOrPhoneRequired = Error.Validation(
        "User.EmailOrPhoneRequired",
        "Either email or phone number must be provided."
    );

    public static readonly Error EmailNotProvided = Error.Validation(
        "User.EmailNotProvided",
        "Email must be provided to verify email."
    );

    public static readonly Error PhoneNotProvided = Error.Validation(
        "User.PhoneNotProvided",
        "Phone number must be provided to verify phone."
    );

    public static readonly Error InvalidPassword = Error.Validation(
        "User.InvalidPassword",
        "The password does not meet the complexity requirements."
    );

    public static readonly Error InvalidLockDuration = Error.Validation(
        "User.InvalidLockDuration",
        "The lock duration must be greater than zero."
    );

    public static readonly Error TooManyIdentities = Error.Validation(
        "User.TooManyIdentities",
        "User cannot have more than 2 identity providers."
    );


    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "User.EmailAlreadyExists",
        "A user with this email already exists"
    );

    public static readonly Error PhoneAlreadyExists = Error.Conflict(
        "User.PhoneNumberAlreadyExists",
        "A user with this phone number already exists"
    );
}