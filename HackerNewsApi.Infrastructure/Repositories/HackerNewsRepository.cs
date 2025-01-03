using HackerNewsApi.Domain.Entities;
using HackerNewsApi.Infrastructure.Intefraces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HackerNewsApi.Infrastructure.Repositories;

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
                if (story != null) stories.Add(story);
            }

            _logger.LogInformation("Successfully fetched {Count} stories.", stories.Count);
            return stories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching stories from Hacker News API.");
            throw;
        }
    }
}
