namespace Fayora.Infrastructure.Settings
{
    public class FacebookSettings
    {
        public static string SectionName => "FacebookSettings";
        public string AppId { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
    }
}