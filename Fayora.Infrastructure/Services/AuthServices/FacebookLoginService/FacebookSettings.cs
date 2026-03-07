namespace Fayora.Infrastructure.Services.AuthServices.FacebookLoginService
{
    public class FacebookSettings
    {
        public const string Section = "FacebookSettings";
        public string AppId { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
    }
}