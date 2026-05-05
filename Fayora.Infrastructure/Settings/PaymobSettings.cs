namespace Fayora.Infrastructure.Settings;

public class PaymobSettings
{
    public static readonly string SectionName = "PaymobSettings";
    public string ApiKey { get; set; } = string.Empty;
    public string IntegrationId { get; set; } = string.Empty;
    public string IframeId { get; set; } = string.Empty;
    public string HmacSecret { get; set; } = string.Empty;
}
