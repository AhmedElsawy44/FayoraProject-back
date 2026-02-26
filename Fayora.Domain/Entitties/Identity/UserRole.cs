using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.Identity;

public class UserRole : BaseEntity<int>
{
    public Guid UserId { get; init; }
    public int RoleId { get; init; }
    public DateTimeOffset AssignedAt { get; init; } = DateTimeOffset.UtcNow;

    public UserRole(Guid userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
