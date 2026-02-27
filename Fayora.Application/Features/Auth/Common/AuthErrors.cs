using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.Auth.Common;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = Error.Validation(
        code: "Authentication.InvalidCredentials",
        description: "Invalid credentials");
}