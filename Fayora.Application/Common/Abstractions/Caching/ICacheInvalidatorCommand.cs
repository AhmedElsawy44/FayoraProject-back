namespace Fayora.Application.Common.Abstractions.Caching;

public interface ICacheInvalidatorCommand
{
    string CacheKeyToClear { get; }
}