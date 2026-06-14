using Fayora.Domain.Entities.TouristModule;

namespace Fayora.Application.Common.Interfaces.Persistences.TouristModule;

public interface IMasterInterestRepository
{
    Task<IEnumerable<MasterInterest>> GetAllInterestsAsync();
    Task<bool> InterestsExistAsync(IEnumerable<int> interestIds, CancellationToken cancellationToken);
    Task<List<Fayora.Contracts.AdminModule.MasterInterests.GetMasterInterestsResponse>> GetMasterInterestsAsync(CancellationToken ct);
    Task<MasterInterest?> GetByIdAsync(int id, CancellationToken ct);
    void Add(MasterInterest interest);
    void Remove(MasterInterest interest);
}

