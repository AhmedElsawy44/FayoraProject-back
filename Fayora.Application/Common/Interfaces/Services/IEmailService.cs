using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
