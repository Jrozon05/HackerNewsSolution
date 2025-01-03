using HackerNewsApi.Application.Interfaces;
using HackerNewsApi.Domain.Entities;
using HackerNewsApi.Infrastructure.Cache;
using HackerNewsApi.Infrastructure.Intefraces;
using HackerNewsApi.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Application.Services;

public class HackerNewsService : IHackerNewsService
{
    private readonly ICacheManager _cacheManager;
    private readonly IHackerNewsRepository _repository;
    private readonly ILogger<HackerNewsService> _logger;
    private const string CacheKey = "NewestStories";

    public HackerNewsService(ICacheManager cacheManager, IHackerNewsRepository repository, ILogger<HackerNewsService> logger)
    {
        _cacheManager = cacheManager;
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Story>> GetNewestStoriesAsync()
    {
        try
        {
            _logger.LogInformation("Checking cache for key: {CacheKey}", CacheKey);
            return await _cacheManager.GetOrCreateAsync(CacheKey, async () =>
            {
                _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from repository.", CacheKey);
                return await _repository.GetNewestStoriesAsync();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching newest stories.");
            throw;
        }
    }
}
