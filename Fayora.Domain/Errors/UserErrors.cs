using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;

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

    public static readonly Error TooManyIdentities = Error.Validation(
        "User.TooManyIdentities",
        $"User cannot have more than {User.MaxUserIdentities} identity providers."
    );


    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "User.EmailAlreadyExists",
        "A user with this email already exists"
    );

    public static readonly Error PhoneAlreadyExists = Error.Conflict(
        "User.PhoneNumberAlreadyExists",
        "A user with this phone number already exists"
    );

    public static readonly Error DeviceIdMissing = Error.Validation(
        "User.DeviceIdMissing",
        "Device ID is missing."
    );

    public static readonly Error DeviceBanned = Error.Failure(
        "User.DeviceBanned",
        "This device has been banned."
    );

    public static readonly Error EmailBanned = Error.Failure(
        "User.EmailBanned",
        "This email address has been banned."
    );

    public static readonly Error PhoneBanned = Error.Failure(
        "User.PhoneBanned",
        "This phone number has been banned."
    );

    public static readonly Error OtpCooldownNotMet = Error.Failure(
        code: "User.OtpCooldownNotMet",
        description: $"Please wait at least {User.OtpResendCooldown.TotalMinutes} minutes before requesting a new verification code."
    );

    public static readonly Error DailyOtpLimitReached = Error.Failure(
        code: "User.DailyOtpLimitReached",
        description: "You have reached the maximum number of verification codes allowed per day. Please try again after 24 hours."
    );

    public static readonly Error OnlyOneAllowed = Error.Validation(
        code: "User.OnlyOneAllowed",
        description: "You can provide either Email or Phone Number, not both."
        );

    public static readonly Error InvalidOrExpiredOtp = Error.NotFound(
        code: "VerificationCode.InvalidOrExpired",
        description: "The verification code is invalid, expired, or has not been requested."
    );

    public static readonly Error AccountAlreadyVerified = Error.Conflict(
        code: "User.AccountAlreadyVerified",
        description: "The account is already verified."
    );
}