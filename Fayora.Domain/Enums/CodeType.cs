using Ardalis.SmartEnum;

namespace Fayora.Domain.Enums;

public class CodeType : SmartEnum<CodeType>
{
    private CodeType(string name, int value) : base(name, value) { }

    public static readonly CodeType Email = new CodeType("Email", 1);
    public static readonly CodeType SMS = new CodeType("SMS", 2);
}
