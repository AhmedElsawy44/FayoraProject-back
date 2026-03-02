using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Presistance;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetUserCode(Guid userId, string target, OtpPurpose purpose, CancellationToken cancellationToken);
}
