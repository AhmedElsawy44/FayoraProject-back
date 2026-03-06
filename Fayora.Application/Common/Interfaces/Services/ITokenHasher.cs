namespace Fayora.Application.Common.Interfaces.Services;

public interface ITokenHasher
{
    string HashToken(string token);
}
