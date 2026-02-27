using Fayora.Domain.Common.Results;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IPasswordHasher
{
    Result<string> HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
