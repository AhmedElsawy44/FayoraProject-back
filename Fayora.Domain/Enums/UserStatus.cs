using Ardalis.SmartEnum;

namespace Fayora.Domain.Enums;

public class UserStatus : SmartEnum<UserStatus>
{
    public static readonly UserStatus Active = new(nameof(Active), 1);
    public static readonly UserStatus Locked = new(nameof(Locked), 2);
    public static readonly UserStatus Deleted = new(nameof(Deleted), 3);

    private UserStatus(string name, int value) : base(name, value) { }
}

