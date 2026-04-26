namespace Fayora.Application.Common.Interfaces.Persistences.SharedModule;

public interface ICityRepository
{
    Task<bool> CitiesExistsAsync(List<int> cityIds, CancellationToken cancellationToken);
}
