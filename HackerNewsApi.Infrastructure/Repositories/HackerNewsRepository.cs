using HackerNewsApi.Domain.Entities;
using HackerNewsApi.Infrastructure.Intefraces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HackerNewsApi.Infrastructure.Repositories
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsRepository> _logger;

        public HackerNewsRepository(HttpClient httpClient, ILogger<HackerNewsRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching story IDs from Hacker News API.");
                var storyIds = await _httpClient.GetFromJsonAsync<IEnumerable<int>>("https://hacker-news.firebaseio.com/v0/newstories.json");

                var stories = new List<Story>();
                foreach (var id in storyIds.Take(10))
                {
                    _logger.LogInformation("Fetching details for story ID: {StoryId}", id);
                    var story = await _httpClient.GetFromJsonAsync<Story>($"https://hacker-news.firebaseio.com/v0/item/{id}.json");

                    // Only add valid stories with a URL
                    if (story != null && !string.IsNullOrEmpty(story.Url))
                    {
                        stories.Add(story);
                    }
                }

                _logger.LogInformation("Fetched {Count} valid stories.", stories.Count);
                return stories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stories from Hacker News API.");
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetStoryIdsAsync()
        {
            _logger.LogInformation("Fetching story IDs from Hacker News API.");
            var storyIds = await _httpClient.GetFromJsonAsync<IEnumerable<int>>("https://hacker-news.firebaseio.com/v0/newstories.json");
            _logger.LogInformation("Fetched {Count} story IDs.", storyIds?.Count() ?? 0);
            return storyIds ?? new List<int>();
        }

        public async Task<Story> GetStoryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching story details for ID: {StoryId}", id);
            var story = await _httpClient.GetFromJsonAsync<Story>($"https://hacker-news.firebaseio.com/v0/item/{id}.json");
            if (story == null)
            {
                _logger.LogWarning("Story with ID: {StoryId} not found.", id);
            }
            return story;
        }
    }
}
