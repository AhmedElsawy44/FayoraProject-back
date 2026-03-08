using Fayora.Domain.Common.Results;
using Fayora.Domain.Common.ValueObjects;
using Fayora.Domain.Errors;
using System.Text.RegularExpressions;

namespace Fayora.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value.ToLowerInvariant().Trim();

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return UserErrors.InvalidEmail;

        if (!EmailRegex.IsMatch(email))
            return UserErrors.InvalidEmail;

        return new Email(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}