using Microsoft.OpenApi.Models;

namespace HackerNewsApi.Api;

public static class SwaggerConfig
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "HackerNewsApi", Version = "v1" });
        });
    }

    public static void UseSwaggerUIConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNewsApi v1"));
    }
}
