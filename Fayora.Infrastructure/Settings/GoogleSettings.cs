namespace Fayora.Infrastructure.Settings;

public class GoogleSettings
{
    public static string SectionName => "GoogleSettings";
    public string ClientId { get; set; } = string.Empty;
}
