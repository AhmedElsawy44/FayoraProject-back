using Fayora.Domain.Common.Results;
using Fayora.Domain.Common.ValueObject;

namespace Fayora.Domain.ValueObjects;

public class FileUrl : ValueObject
{
    public string Value { get; init; }

    private FileUrl(string value)
    {
        Value = value;
    }

    public static Result<FileUrl> Create(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Error.Validation(
                code: "FileUrl.Required",
                description: "File URL is required.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            return Error.Validation(
                code: "FileUrl.Invalid",
                description: "File URL must be a valid absolute URL that starts with http or https.");
        }

        return new FileUrl(url);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents() => [Value];
}