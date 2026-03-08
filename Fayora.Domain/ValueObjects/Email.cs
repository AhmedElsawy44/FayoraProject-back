using Fayora.Domain.Common.Results;
using Fayora.Domain.Errors;

namespace Fayora.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value) => Value = value.ToLowerInvariant().Trim();

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return UserErrors.InvalidEmail;

        if (!email.Contains('@'))
            return UserErrors.InvalidEmail;

        return new Email(email);
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);
    public override string ToString() => Value;
}