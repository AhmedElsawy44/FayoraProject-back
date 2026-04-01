using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Contracts.AuthModule.ConfirmChangePhone;

public record ConfirmChangePhoneRequest
(
    string NewPhoneNumber,
    string Code
);
