using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Domain.ValueObjects;

public class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value.ToLower();
    }

    public static Result<Email> Create(string email)
    {
        if (!email.Contains("@"))
            return UserErrors.InvalidEmail;

        return new Email(email);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (obj is Email email) return Value != email.Value;

        return true;
    }
    public static bool operator ==(Email? left, Email? right) => left?.Value == right?.Value;
    public static bool operator !=(Email? left, Email? right) => !(left == right);
}
