namespace Fayora.Domain.Entities.IdentityModule;

public class Role : BaseEntity<int>
{
    public string Name { get; private set; } = string.Empty;

    public Role(string name)
    {
        Name = name;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    private Role() { }
}
