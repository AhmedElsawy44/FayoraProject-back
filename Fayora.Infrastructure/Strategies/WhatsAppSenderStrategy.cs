using Fayora.Application.Common.Strategies;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Fayora.Infrastructure.Strategies;

public class WhatsAppSenderStrategy(IOptions<TwilioSettings> options, ILogger<WhatsAppSenderStrategy> logger) : IMessageSenderStrategy
{
    private readonly TwilioSettings _settings = options.Value;

    public CodeDeliveryMethod Method => CodeDeliveryMethod.WhatsApp;

    public async Task<bool> SendAsync(string to, string message)
    {
        try
        {
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);

            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber($"whatsapp:{_settings.FromWhatsAppNumber}"),
                to: new PhoneNumber($"whatsapp:{to}")
            );

            logger.LogInformation("✅ WhatsApp sent successfully. SID: {Sid}", messageResource.Sid);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Failed to send WhatsApp to {Phone}", to);
            return false;
        }
    }
}