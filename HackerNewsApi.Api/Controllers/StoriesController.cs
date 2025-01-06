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

    [HttpGet("search")]
    public async Task<IActionResult> SearchStories([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Search query is empty or null.");
                return BadRequest("Query parameter cannot be empty.");
            }

            _logger.LogInformation("Searching stories for query: {Query}", query);
            var stories = await _hackerNewsService.SearchStoriesAsync(query);
            return Ok(stories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for stories.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedStories([FromQuery] int page, [FromQuery] int pageSize)
    {
        try
        {
            if (page < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters. Page: {Page}, PageSize: {PageSize}", page, pageSize);
                return BadRequest("Page and PageSize must be greater than 0.");
            }

            _logger.LogInformation("Fetching paged stories. Page: {Page}, PageSize: {PageSize}", page, pageSize);
            var stories = await _hackerNewsService.GetPagedStoriesAsync(page, pageSize);
            return Ok(stories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching paged stories.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
