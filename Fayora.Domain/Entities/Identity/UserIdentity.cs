using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entitties.Identity;

public class UserIdentity : BaseEntity<int>
{
    public Guid UserId { get; init; }
    public IdentityProvider Provider { get; init; }
    public string ProviderKey { get; init; } = string.Empty;
    public Email? Email { get; private set; }
    public DateTimeOffset LinkedAt { get; init; } = DateTimeOffset.UtcNow;

    public UserIdentity(Guid userId, IdentityProvider provider, string providerKey, Email? email)
    {
        UserId = userId;
        Provider = provider;
        ProviderKey = providerKey;
        Email = email;
    }

    public void UpdateProfile(Email email) => Email = email;

    private UserIdentity() { }
}