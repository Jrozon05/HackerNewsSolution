using HackerNewsApi.Domain.Entities;

namespace HackerNewsApi.Application.Interfaces;

public interface IHackerNewsService
{
    Task<IEnumerable<Story>> GetNewestStoriesAsync();
    Task<IEnumerable<Story>> SearchStoriesAsync(string query);
    Task<IEnumerable<Story>> GetPagedStoriesAsync(int page, int pageSize);

}
