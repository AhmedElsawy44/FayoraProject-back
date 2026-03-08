using System.ComponentModel.DataAnnotations;

namespace Fayora.Infrastructure.Settings;

public class EmailSettings
{
    [Required]
    public string Sender { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; }
}