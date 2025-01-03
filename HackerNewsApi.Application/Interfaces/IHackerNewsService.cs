using HackerNewsApi.Domain.Entities;

namespace HackerNewsApi.Application.Interfaces;

public interface IHackerNewsService
{
    Task<IEnumerable<Story>> GetNewestStoriesAsync();
}
