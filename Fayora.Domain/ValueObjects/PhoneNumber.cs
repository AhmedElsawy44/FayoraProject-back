using Fayora.Domain.Common.Results;
using Fayora.Domain.Common.ValueObjects;
using System.Text.RegularExpressions;

namespace Fayora.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{

    private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(
                code: "PhoneNumber.Required",
                description: "Phone number cannot be empty.");

        var cleanedValue = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (!PhoneRegex.IsMatch(cleanedValue))
            return Error.Validation(
                code: "PhoneNumber.InvalidFormat",
                description: "Invalid phone number format. Please provide a valid international phone number.");

        return new PhoneNumber(cleanedValue);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Value];
    }
}