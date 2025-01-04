using HackerNewsApi.Api.Extensions;
using HackerNewsApi.Application.Interfaces;
using HackerNewsApi.Application.Services;
using HackerNewsApi.Infrastructure.Cache;
using HackerNewsApi.Infrastructure.Intefraces;
using HackerNewsApi.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("C:/Users/jrozo/Repository/HackerNewsSolution/logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Allows requests from any origin
              .AllowAnyMethod() // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allows any HTTP headers
    });
});

// Dependency Injection
builder.Services.ConfigureDependencies();

// Enable controllers
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HackerNewsApi",
        Version = "v1",
        Description = "API to fetch the newest stories from Hacker News."
    });
});

var app = builder.Build();

// Configure middleware
app.UseMiddleware<HackerNewsApi.Api.Middlewares.ExceptionHandlingMiddleware>();

app.UseCors("AllowAll");
// Error handling middleware
app.UseExceptionHandler("/error"); // Optional: Create an error handling endpoint
app.UseHsts(); // Enforce strict transport security

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNewsApi v1");
        c.RoutePrefix = string.Empty;
    });
}

// Enable routing and endpoints
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
