using Fayora.Application.Common.Strategies;

namespace Fayora.Application.Common.Factories;

public class UploadStrategyFactory(IEnumerable<IUploadStrategy> strategies)
{
    public IUploadStrategy GetStrategy(UploadContext context)
    {
        var strategy = strategies.FirstOrDefault(s => s.Context == context);

        return strategy ?? throw new InvalidOperationException($"No upload strategy found for context: {context}");
    }
}
