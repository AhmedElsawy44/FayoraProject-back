using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.Auth.Common;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = Error.Validation(
        code: "Authentication.InvalidCredentials",
        description: "Invalid credentials"
    );

    public static readonly Error UserAccountIsAlreadyVerified = Error.Conflict(
        code: "User.AccountAlreadyVerified",
        description: "The user account is already verified."
    );

    public static readonly Error UserNotFound = Error.NotFound(
        code: "Authentication.UserNotFound",
        description: "User not found."
    );

    public static readonly Error UserNotVerified = Error.Validation(
        code: "Authentication.UserNotVerified",
        description: "User account is not verified."
    );

    public static readonly Error InvalidRefreshToken = Error.Validation(
       code: "Authentication.InvalidRefreshToken",
       description: "Invalid refresh token."
   );

    public static readonly Error InvalidResetToken = Error.Validation(
       code: "Authentication.InvalidResetToken",
       description: "Invalid password reset token."
   );

    public static readonly Error InvalidVerificationCode = Error.Validation(
       code: "Authentication.InvalidVerificationCode",
       description: "Invalid verification code."
    );

    public static readonly Error DeviceBanned = Error.Failure(
        "Authentication.DeviceBanned",
        "This device has been banned."
    );

    public static readonly Error UserDeleted = Error.Failure(
        "Authentication.UserDeleted",
        "This user account has been deleted."
    );

    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "Authentication.EmailAlreadyExists",
        "A user with this email already exists"
    );

    public static readonly Error PhoneAlreadyExists = Error.Conflict(
        "Authentication.PhoneNumberAlreadyExists",
        "A user with this phone number already exists"
    );

    public static readonly Error AccountAlreadyVerified = Error.Conflict(
        code: "Authentication.AccountAlreadyVerified",
        description: "The account is already verified."
    );

    public static readonly Error EmailIsAlreadyVerified = Error.Conflict(
        code: "Authentication.EmailAlreadyVerified",
        description: "The email is already verified."
    );

    public static readonly Error PhoneIsAlreadyVerified = Error.Conflict(
        code: "Authentication.PhoneAlreadyVerified",
        description: "The phone number is already verified."
    );

    public static readonly Error EmailRequiredFromApple = Error.Validation(
        code: "Authentication.EmailRequiredFromApple",
        description: "Email is required from Apple. Please register again and provide email permission."
    );
}