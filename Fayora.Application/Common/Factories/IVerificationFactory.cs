using Fayora.Application.Common.Strategies;

namespace Fayora.Application.Common.Factories;

public interface IVerificationFactory
{
    IVerificationStrategy? GetStrategy(string entityType);
}
