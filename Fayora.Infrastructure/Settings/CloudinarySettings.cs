using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Infrastructure.Settings;

public class CloudinarySettings
{
    public const string SectionName = "CloudinarySettings";
    public string CloudName { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
    public string ApiSecret { get; init; } = null!;
}
