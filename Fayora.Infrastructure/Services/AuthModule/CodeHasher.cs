using Fayora.Domain.Common.Interfaces.IdentityModule;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services.AuthModule;

public class CodeHasher : ICodeHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    public string HashCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            code,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyCode(string code, string codeHash)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(codeHash))
            return false;

        try
        {
            byte[] hashBytes = Convert.FromBase64String(codeHash);

            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            byte[] expectedHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, expectedHash, 0, HashSize);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                code,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }
}