using Microsoft.AspNetCore.Mvc;
using HackerNewsApi.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HackerNewsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoriesController : ControllerBase
{
    private readonly IHackerNewsService _hackerNewsService;
    private readonly ILogger<StoriesController> _logger;

    public StoriesController(IHackerNewsService hackerNewsService, ILogger<StoriesController> logger)
    {
        _hackerNewsService = hackerNewsService;
        _logger = logger;
    }

    [HttpGet("newest")]
    public async Task<IActionResult> GetNewestStories()
    {
        try
        {
            _logger.LogInformation("Fetching newest stories from the Hacker News API.");
            var stories = await _hackerNewsService.GetNewestStoriesAsync();
            return Ok(stories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching newest stories.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
