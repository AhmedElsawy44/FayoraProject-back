using Fayora.Domain.Common.Interfaces.IdentityModule;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services.AuthServices;

public class VerificationCodeService : IVerificationCodeService
{
    public string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}
