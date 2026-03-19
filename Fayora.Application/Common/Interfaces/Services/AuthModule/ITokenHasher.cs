namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface ITokenHasher
{
    string HashToken(string token);
}
