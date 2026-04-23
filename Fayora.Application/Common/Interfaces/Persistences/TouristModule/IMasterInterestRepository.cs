using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Common.Interfaces.Persistences.TouristModule;

public interface IMasterInterestRepository
{
    Task<IEnumerable<MasterInterest>> GetAllInterestsAsync();
    Task<bool> InterestsExistAsync(IEnumerable<int> interestIds, CancellationToken cancellationToken);
}
