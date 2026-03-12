using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Accommodation.Commands.CreateHousingOwner;

public record CreateHousingOwnerResult(
    Guid UserId,
    string Token,
    string RefreshToken,
    int ExpiresIn);
