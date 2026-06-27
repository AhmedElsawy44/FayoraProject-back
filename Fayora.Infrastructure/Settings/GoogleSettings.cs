using System.Collections.Generic;

namespace Fayora.Infrastructure.Settings;

public class GoogleSettings
{
    public static string SectionName => "GoogleSettings";
    public List<string> ClientIds { get; set; } = new();
}
