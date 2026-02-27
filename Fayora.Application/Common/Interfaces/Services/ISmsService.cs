using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Services;

public interface ISmsService
{
    Task SendSMSAsync(string phoneNumber, string text);
}
