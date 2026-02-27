using Fayora.Domain.Common.Results;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IPasswordHasher
{
    Result<string> Hash(string password);
    string HashVerificationCode(string code);
    bool Verify(string password, string passwordHash);
}
