using HackerNewsApi.Application.Interfaces;
using HackerNewsApi.Application.Services;
using HackerNewsApi.Domain.Entities;
using HackerNewsApi.Infrastructure.Cache;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using HackerNewsApi.Infrastructure.Intefraces;

namespace HackerNewsApi.Tests.UnitTests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<ICacheManager> _mockCacheManager;
        private readonly Mock<IHackerNewsRepository> _mockRepository;
        private readonly Mock<ILogger<HackerNewsService>> _mockLogger;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _mockCacheManager = new Mock<ICacheManager>();
            _mockRepository = new Mock<IHackerNewsRepository>();
            _mockLogger = new Mock<ILogger<HackerNewsService>>();
            _service = new HackerNewsService(
                _mockCacheManager.Object,
                _mockRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsStories_WhenCacheIsEmpty()
        {
            // Arrange
            var fakeStories = new List<Story>
            {
                new Story { Id = 1, Title = "Story 1", Url = "https://example.com/1" },
                new Story { Id = 2, Title = "Story 2", Url = "https://example.com/2" }
            };

            _mockCacheManager
                .Setup(m => m.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Story>>>>()))
                .ReturnsAsync(fakeStories);

            // Act
            var result = await _service.GetNewestStoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.Title == "Story 1");
        }

        [Fact]
        public async Task GetNewestStoriesAsync_FiltersOutStoriesWithoutUrls()
        {
            // Arrange
            var fakeStories = new List<Story>
            {
                new Story { Id = 1, Title = "Story 1", Url = "https://example.com/1" },
                new Story { Id = 2, Title = "Story 2", Url = null }, // Invalid URL
                new Story { Id = 3, Title = "Story 3", Url = "" }    // Empty URL
            };

            _mockCacheManager
                .Setup(m => m.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Story>>>>()))
                .ReturnsAsync(fakeStories);

            // Act
            var result = await _service.GetNewestStoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only 1 valid story with a URL
            Assert.Equal("Story 1", result.First().Title);
        }
    }
}
