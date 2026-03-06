using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistances;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCodeAsync(Guid userId, string identifier, OtpPurpose purpose, CancellationToken cancellationToken, bool isTracking = true);
}
