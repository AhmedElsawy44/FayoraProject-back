using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entitties.Identity;

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
