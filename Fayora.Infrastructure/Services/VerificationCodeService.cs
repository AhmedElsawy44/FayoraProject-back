using Fayora.Application.Common.Interfaces.Services;
using System.Security.Cryptography;

namespace Fayora.Infrastructure.Services;

public class VerificationCodeService : IVerificationCodeService
{
    public string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}
