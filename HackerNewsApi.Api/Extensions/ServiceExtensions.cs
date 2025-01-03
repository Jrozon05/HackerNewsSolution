using HackerNewsApi.Application.Interfaces;
using HackerNewsApi.Application.Services;
using HackerNewsApi.Infrastructure.Repositories;
using HackerNewsApi.Infrastructure.Cache;
using HackerNewsApi.Infrastructure.Intefraces;

namespace HackerNewsApi.Api.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDependencies(this IServiceCollection services)
    {
        services.AddScoped<IHackerNewsService, HackerNewsService>();
        services.AddScoped<IHackerNewsRepository, HackerNewsRepository>();
        services.AddSingleton<ICacheManager, CacheManager>();
    }
}
