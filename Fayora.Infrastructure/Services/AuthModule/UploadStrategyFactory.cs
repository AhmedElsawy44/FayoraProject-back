using Fayora.Application.Common.Strategies;

namespace Fayora.Infrastructure.Services.AuthModule;

public class UploadStrategyFactory(IEnumerable<IUploadStrategy> strategies)
{
    public IUploadStrategy GetStrategy(UploadContext context)
    {
        var strategy = strategies.FirstOrDefault(s => s.Context == context);

        if (strategy == null)
            throw new InvalidOperationException($"No upload strategy found for context: {context}");

        return strategy;
    }
}
