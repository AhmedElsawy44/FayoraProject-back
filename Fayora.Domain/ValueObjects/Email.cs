using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Domain.ValueObjects;

public class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string email)
    {
        if (!email.Contains("@"))
            return UserErrors.InvalidEmail;

        return new Email(email);
    }
}
