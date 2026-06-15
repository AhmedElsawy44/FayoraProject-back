using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.AccommodationModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.AccommodationModule;

public class MasterAmenity : BaseEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public FileUrl Icon { get; private set; } = default!;
    public AmenityCategory Category { get; private set; }
    public bool IsActive { get; private set; } = true;

    private MasterAmenity() { }

    public static Result<MasterAmenity> Create(string name, AmenityCategory category, string? icon = null)
    {
        var iconUrlResult = FileUrl.Create(icon);
        if (iconUrlResult.IsError) return iconUrlResult.Errors;

        return new MasterAmenity
        {
            Name = name,
            Category = category,
            Icon = iconUrlResult.Value
        };
    }

    public Result<Success> Update(string name, AmenityCategory category, string? icon)
    {
        var iconUrlResult = FileUrl.Create(icon);
        if (iconUrlResult.IsError) return iconUrlResult.Errors;

        Name = name;
        Category = category;
        Icon = iconUrlResult.Value;

        return Result.Success;
    }

    public void ToggleStatus()
    {
        IsActive = !IsActive;
    }
}