namespace Fayora.Application.Common.Authorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    public string? Roles { get; set; }
}