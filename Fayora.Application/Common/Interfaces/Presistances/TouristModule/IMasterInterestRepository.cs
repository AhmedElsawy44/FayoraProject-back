using Fayora.Domain.Entitties.Tourist;

namespace Fayora.Application.Common.Interfaces.Presistances.TouristModule;

public interface IMasterInterestRepository
{
    Task<IEnumerable<MasterInterest>> GetAllInterestsAsync();
    Task<bool> InterestsExistAsync(IEnumerable<int> interestIds, CancellationToken cancellationToken);
}
