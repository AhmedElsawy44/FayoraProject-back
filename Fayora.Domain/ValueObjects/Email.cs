using ErrorOr;
using Fayora.Domain.Errors;

namespace Fayora.Domain.ValueObjects;

public class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static ErrorOr<Email> Create(string email)
    {
        if (!email.Contains("@"))
            return UserErrors.InvalidEmail;

        return new Email(email);
    }
}
