using HackerNewsApi.Application.Interfaces;
using HackerNewsApi.Domain.Entities;
using HackerNewsApi.Infrastructure.Cache;
using HackerNewsApi.Infrastructure.Intefraces;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Application.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IHackerNewsRepository _repository;
        private readonly ILogger<HackerNewsService> _logger;

        private const string NewestStoriesCacheKey = "NewestStories";
        private const string AllStoryIdsCacheKey = "AllStoryIds";

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
                _logger.LogInformation("Fetching newest stories.");

                // Retrieve the newest stories from cache or repository
                var newestStories = await _cacheManager.GetOrCreateAsync(NewestStoriesCacheKey, TimeSpan.FromMinutes(5), async () =>
                {
                    var storyIds = await GetAllStoryIdsAsync();
                    return await FetchStoriesByIdsAsync(storyIds);
                });

                _logger.LogInformation("Returning {Count} newest stories.", newestStories.Count());
                return newestStories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching newest stories.");
                throw;
            }
        }

        public async Task<IEnumerable<Story>> GetPagedStoriesAsync(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
                return Enumerable.Empty<Story>();

            try
            {
                _logger.LogInformation("Fetching paged stories. Page: {Page}, PageSize: {PageSize}", page, pageSize);

                // Retrieve all story IDs from cache or repository
                var storyIds = await _cacheManager.GetOrCreateAsync(AllStoryIdsCacheKey, TimeSpan.FromMinutes(1), GetAllStoryIdsAsync);

                // Fetch stories for the requested page
                var pagedStoryIds = storyIds.Skip((page - 1) * pageSize).Take(pageSize);
                var pagedStories = await FetchStoriesByIdsAsync(pagedStoryIds);

                _logger.LogInformation("Returning {Count} stories for Page: {Page}, PageSize: {PageSize}.", pagedStories.Count(), page, pageSize);
                return pagedStories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paged stories.");
                throw;
            }
        }

        public async Task<IEnumerable<Story>> SearchStoriesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Story>();

            try
            {
                _logger.LogInformation("Searching for stories matching query: {Query}", query);

                var stories = await GetNewestStoriesAsync(); // Retrieve cached stories
                var filteredStories = stories
                    .Where(story => !string.IsNullOrEmpty(story.Title) && story.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _logger.LogInformation("Found {Count} stories matching query: {Query}", filteredStories.Count, query);
                return filteredStories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for stories.");
                throw;
            }
        }

        private async Task<IEnumerable<int>> GetAllStoryIdsAsync()
        {
            _logger.LogInformation("Fetching all story IDs from Hacker News API.");
            var storyIds = await _repository.GetStoryIdsAsync();
            _logger.LogInformation("Fetched {Count} story IDs.", storyIds.Count());
            return storyIds;
        }

        private async Task<IEnumerable<Story>> FetchStoriesByIdsAsync(IEnumerable<int> storyIds)
        {
            // Fetch stories in parallel
            var fetchTasks = storyIds.Select(id => _repository.GetStoryByIdAsync(id));
            var stories = await Task.WhenAll(fetchTasks);

            // Filter out invalid stories
            return stories.Where(story => story != null && !string.IsNullOrEmpty(story.Url));
        }
    }
}
