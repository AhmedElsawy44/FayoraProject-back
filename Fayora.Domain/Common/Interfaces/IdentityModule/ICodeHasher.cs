namespace Fayora.Domain.Common.Interfaces.IdentityModule;

public interface ICodeHasher
{
    string HashCode(string code);
    bool VerifyCode(string code, string codeHash);
}
