using Fayora.Domain.Common.Results;

namespace Fayora.Domain.Common.Interfaces;

public interface IPasswordHasher
{
    Result<string> HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
