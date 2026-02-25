using Fayora.Domain.Common.Results;

namespace Fayora.Application.Common.Authentication;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials = Error.Validation(
        code: "Authentication.InvalidCredentials",
        description: "Invalid credentials");
}
