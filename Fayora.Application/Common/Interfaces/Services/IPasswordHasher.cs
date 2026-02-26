using Fayora.Domain.Common.Results;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IPasswordHasher
{
    Result<string> Hash(string password);
    bool Verify(string password, string passwordHash);
}
