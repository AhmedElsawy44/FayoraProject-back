namespace Fayora.Infrastructure.Settings;

public class TwilioSettings
{
    public const string SectionName = "TwilioSettings";

    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromSmsNumber { get; set; } = string.Empty;
    public string FromWhatsAppNumber { get; set; } = string.Empty;
}