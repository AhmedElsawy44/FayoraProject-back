namespace Fayora.Domain.Common.Results;

public class Error
{
    public string Code { get; set; }
    public string Description { get; set; }
    public ErrorType ErrorType { get; set; }

    private Error(string code, string description, ErrorType errorType)
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public static Error Validation(string code = "General.Validation", string description = "A validation error has occurred.")
    {
        return new Error(code, description, ErrorType.Validation);
    }

    public static Error Failure(string code = "General.Failure", string description = "A failure has occurred.")
    {
        return new Error(code, description, ErrorType.Failure);
    }

    public static Error Unexpected(string code = "General.Unexpected", string description = "An unexpected error has occurred.")
    {
        return new Error(code, description, ErrorType.Unexpected);
    }

    public static Error Conflict(string code = "General.Conflict", string description = "A conflict has occurred.")
    {
        return new Error(code, description, ErrorType.Conflict);
    }

    public static Error NotFound(string code = "General.NotFound", string description = "A not found error has occurred.")
    {
        return new Error(code, description, ErrorType.NotFound);
    }

    public static Error Unauthorized(string code = "General.Unauthorized", string description = "An unauthorized error has occurred.")
    {
        return new Error(code, description, ErrorType.Unauthorized);
    }

    public static Error Forbidden(string code = "General.Forbidden", string description = "A forbidden error has occurred.")
    {
        return new Error(code, description, ErrorType.Forbidden);
    }

    public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "Invalid credentials have been provided.")
    {
        return new Error(code, description, ErrorType.InvalidCredentials);
    }
}
