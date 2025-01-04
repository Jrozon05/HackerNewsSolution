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
        // Register application services
        services.AddScoped<IHackerNewsService, HackerNewsService>();
        services.AddScoped<IHackerNewsRepository, HackerNewsRepository>();

        // Register infrastructure services
        services.AddSingleton<ICacheManager, CacheManager>();
        services.AddMemoryCache(); // Ensure IMemoryCache is registered
        services.AddHttpClient<HackerNewsRepository>();
    }
}
