using Fayora.Domain.Entitties.Tourist;

namespace Fayora.Application.Common.Interfaces.Presistances.TouristModule;

public interface IMasterInterestRepository
{
    Task<IEnumerable<MasterInterest>> GetAllInterestsAsync();
}
