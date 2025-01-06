using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Infrastructure.Cache;

public interface ICacheManager
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
    Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> factory);
    void Set<T>(string key, T value, TimeSpan expiration);
    bool TryGetValue<T>(string key, out T value); 
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

    public bool TryGetValue<T>(string key, out T value)
    {
        var exists = _cache.TryGetValue(key, out value);
        if (exists)
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", key);
        }
        else
        {
            _logger.LogInformation("Cache miss for key: {CacheKey}", key);
        }

        return exists;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });

        _logger.LogInformation("Cache set for key: {CacheKey} with expiration: {Expiration}", key, expiration);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
    {
        if (TryGetValue(key, out T value))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", key);
            return value;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching new value.", key);

        value = await factory();
        Set(key, value, TimeSpan.FromMinutes(1));
        return value;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> factory)
    {
        if (TryGetValue(key, out T value))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", key);
            return value;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching new value.", key);

        value = await factory();
        Set(key, value, expiration);
        return value;
    }
}