using HackerNewsApi.Api.Controllers;
using HackerNewsApi.Application.Interfaces;
using HackerNewsApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HackerNewsApi.Tests.UnitTests
{
    public class StoriesControllerTests
    {
        private readonly Mock<IHackerNewsService> _mockService;
        private readonly Mock<ILogger<StoriesController>> _mockLogger;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _mockService = new Mock<IHackerNewsService>();
            _mockLogger = new Mock<ILogger<StoriesController>>();
            _controller = new StoriesController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetNewestStories_ReturnsOkResult_WithStories()
        {
            // Arrange
            var fakeStories = new List<Story>
            {
                new Story { Id = 1, Title = "Story 1", Url = "https://example.com/1" },
                new Story { Id = 2, Title = "Story 2", Url = "https://example.com/2" }
            };

            _mockService
                .Setup(s => s.GetNewestStoriesAsync())
                .ReturnsAsync(fakeStories);

            // Act
            var result = await _controller.GetNewestStories() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var stories = result.Value as IEnumerable<Story>;
            Assert.Equal(2, stories.Count());
        }

        [Fact]
        public async Task GetNewestStories_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var testException = new System.Exception("Test exception");

            _mockService
                .Setup(s => s.GetNewestStoriesAsync())
                .ThrowsAsync(testException);

            // Act
            var result = await _controller.GetNewestStories() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("An error occurred while processing your request.", result.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while fetching newest stories.")),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchStories_ReturnsOkResult_WithMatchingStories()
        {
            // Arrange
            var query = "Tech";
            var fakeStories = new List<Story>
            {
                new Story { Id = 1, Title = "Tech Advances in 2025", Url = "https://example.com/1" },
                new Story { Id = 2, Title = "New Tech Gadgets Released", Url = "https://example.com/3" }
            };

            _mockService
                .Setup(s => s.SearchStoriesAsync(query))
                .ReturnsAsync(fakeStories);

            // Act
            var result = await _controller.SearchStories(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var stories = result.Value as IEnumerable<Story>;
            Assert.Equal(2, stories.Count());
        }

        [Fact]
        public async Task SearchStories_ReturnsBadRequest_WhenQueryIsEmpty()
        {
            // Act
            var result = await _controller.SearchStories(string.Empty) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Query parameter cannot be empty.", result.Value);
        }

        [Fact]
        public async Task SearchStories_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var query = "Tech";
            _mockService
                .Setup(s => s.SearchStoriesAsync(query))
                .ThrowsAsync(new System.Exception("Test exception"));

            // Act
            var result = await _controller.SearchStories(query) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("An error occurred while processing your request.", result.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while searching for stories.")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetPagedStories_ReturnsOkResult_WithPagedStories()
        {
            // Arrange
            var page = 1;
            var pageSize = 5;

            var fakeStories = Enumerable.Range((page - 1) * pageSize + 1, pageSize)
                .Select(i => new Story { Id = i, Title = $"Story {i}", Url = $"https://example.com/{i}" })
                .ToList();

            // Mock the service to return the fake stories
            _mockService
                .Setup(s => s.GetPagedStoriesAsync(page, pageSize))
                .ReturnsAsync(fakeStories);

            // Act
            var result = await _controller.GetPagedStories(page, pageSize) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var stories = result.Value as IEnumerable<Story>;
            Assert.NotNull(stories);
            Assert.Equal(pageSize, stories.Count());
            Assert.Equal("Story 1", stories.First().Title);
        }


        [Fact]
        public async Task GetPagedStories_ReturnsBadRequest_ForInvalidPageOrPageSize()
        {
            // Act
            var resultInvalidPage = await _controller.GetPagedStories(-1, 10) as BadRequestObjectResult;
            var resultInvalidPageSize = await _controller.GetPagedStories(1, 0) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(resultInvalidPage);
            Assert.Equal(400, resultInvalidPage.StatusCode);
            Assert.Equal("Page and PageSize must be greater than 0.", resultInvalidPage.Value);

            Assert.NotNull(resultInvalidPageSize);
            Assert.Equal(400, resultInvalidPageSize.StatusCode);
            Assert.Equal("Page and PageSize must be greater than 0.", resultInvalidPageSize.Value);
        }

        [Fact]
        public async Task GetPagedStories_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _mockService
                .Setup(s => s.GetPagedStoriesAsync(page, pageSize))
                .ThrowsAsync(new System.Exception("Test exception"));

            // Act
            var result = await _controller.GetPagedStories(page, pageSize) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("An error occurred while processing your request.", result.Value);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while fetching paged stories.")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

    }
}
