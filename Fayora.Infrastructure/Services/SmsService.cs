using Fayora.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Services;

public class SmsService : ISmsService
{
    public Task SendSMSAsync(string phoneNumber, string text)
    {
        throw new NotImplementedException();
    }
}
