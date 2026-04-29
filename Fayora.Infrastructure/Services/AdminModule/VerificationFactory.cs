using Fayora.Application.Common.Factories;
using Fayora.Application.Common.Strategies;

namespace Fayora.Infrastructure.Services.AdminModule;

public class VerificationFactory(IEnumerable<IVerificationStrategy> strategies) : IVerificationFactory
{
    public IVerificationStrategy? GetStrategy(string entityType)
    {
        var strategy = strategies.FirstOrDefault(s => s.CanHandle(entityType));
        return strategy ?? throw new InvalidOperationException($"No verification strategy found for entity type: {entityType}");

    }
}

