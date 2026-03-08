namespace Fayora.Infrastructure.Settings;

public class SmsSettings
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string WhatsAppNumber { get; set; } = string.Empty;
}