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

    }
}
