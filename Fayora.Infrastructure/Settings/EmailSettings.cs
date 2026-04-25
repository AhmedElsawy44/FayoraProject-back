namespace Fayora.Infrastructure.Settings;

public class EmailSettings
{
    public static string SectionName => "EmailSettings";
    public string Sender { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}