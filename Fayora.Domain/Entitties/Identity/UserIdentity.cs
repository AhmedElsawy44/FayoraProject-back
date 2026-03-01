using Fayora.Domain.Enums;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.Identity;

public class UserIdentity : BaseEntity<int>
{
    public Guid UserId { get; init; }
    public IdentityProvider Provider { get; init; }
    public string ProviderKey { get; init; } = string.Empty;
    public Email Email { get; private set; } = Email.Create("").Value;
    public string ProfileData { get; private set; } = string.Empty;
    public DateTimeOffset LinkedAt { get; init; } = DateTimeOffset.UtcNow;

    internal UserIdentity(Guid userId, IdentityProvider provider, string providerKey, Email email, string profileData)
    {
        UserId = userId;
        Provider = provider;
        ProviderKey = providerKey;
        Email = email;
        ProfileData = profileData;
    }

    internal void UpdateProfile(Email email, string profileData)
    {
        Email = email;
        ProfileData = profileData;
    }

    private UserIdentity() { }
}