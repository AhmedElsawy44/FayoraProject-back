using Fayora.Domain.Common.ValueObjects;
using Fayora.Domain.Enums.IdentityModule;

namespace Fayora.Domain.Entities.IdentityModule;

public class UserLanguageProficiency : ValueObject
{
    public decimal ProficiencyLevel { get; private set; }
    public Language Language { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [ProficiencyLevel, Language];
    }

    public UserLanguageProficiency(Language language, decimal proficiencyLevel)
    {
        Language = language;
        ProficiencyLevel = proficiencyLevel;
    }
}
