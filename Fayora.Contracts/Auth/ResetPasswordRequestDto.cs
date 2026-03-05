using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.Auth;

public record ResetPasswordRequestDto(
    string? Email,
    string? PhoneNumber,
    string Token,
    string NewPassword);
