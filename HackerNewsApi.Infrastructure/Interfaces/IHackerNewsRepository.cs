using HackerNewsApi.Domain.Entities;

namespace HackerNewsApi.Infrastructure.Intefraces;

public interface IHackerNewsRepository
{
    Task<IEnumerable<Story>> GetNewestStoriesAsync();
}
