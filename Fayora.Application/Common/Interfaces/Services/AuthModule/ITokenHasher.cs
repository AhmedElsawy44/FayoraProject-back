namespace Fayora.Application.Common.Interfaces.Services.AuthServices;

public interface ITokenHasher
{
    string HashToken(string token);
}
