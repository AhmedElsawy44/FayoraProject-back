namespace Fayora.Application.Common.Interfaces.Services;

public interface ICodeHasher
{
    string HashCode(string code);
    bool VerifyCode(string code, string codeHash);
}
