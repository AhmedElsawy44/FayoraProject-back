using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Fayora.Infrastructure.Strategies;

public class SmsSenderStrategy(IOptions<TwilioSettings> options, ILogger<SmsSenderStrategy> logger) : IMessageSenderStrategy
{
    private readonly TwilioSettings _settings = options.Value;

    public CodeDeliveryMethod Method => CodeDeliveryMethod.Sms;

    public async Task<bool> SendAsync(string to, string message)
    {
        try
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_settings.FromSmsNumber),
                to: new PhoneNumber(to)
            );

            logger.LogInformation("✅ SMS sent successfully. SID: {Sid}", messageResource.Sid);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to send SMS to {Phone}", to);
            return false;
        }
    }
}