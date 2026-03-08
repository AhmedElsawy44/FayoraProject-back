namespace Fayora.Infrastructure.Settings
{
    public class FacebookSettings
    {
        public const string Section = "FacebookSettings";
        public string AppId { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
    }
}