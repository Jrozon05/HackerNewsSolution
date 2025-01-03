using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Infrastructure.Cache;

public interface ICacheManager
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
}

public class CacheManager : ICacheManager
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheManager> _logger;

    public CacheManager(IMemoryCache cache, ILogger<CacheManager> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
    {
        // Check if the item exists in the cache
        if (_cache.TryGetValue(key, out T cachedValue))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", key);
            return cachedValue;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}. Generating new value.", key);

        // Generate the value and cache it
        var value = await factory();
        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        _logger.LogInformation("Cache updated for key: {CacheKey}", key);

        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogInformation("Cache entry removed for key: {CacheKey}", key);
    }
}