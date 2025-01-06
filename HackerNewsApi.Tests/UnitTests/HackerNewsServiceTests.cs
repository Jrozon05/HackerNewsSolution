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
            var storyIds = new List<int> { 1, 2 };
            var stories = new List<Story>
            {
                new Story { Id = 1, Title = "Story 1", Url = "https://example.com/1" },
                new Story { Id = 2, Title = "Story 2", Url = "https://example.com/2" }
            };

            _mockCacheManager
                .Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<Func<Task<IEnumerable<Story>>>>()))
                .ReturnsAsync(stories);

            _mockRepository
                .Setup(r => r.GetStoryIdsAsync())
                .ReturnsAsync(storyIds);

            _mockRepository
                .Setup(r => r.GetStoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => stories.FirstOrDefault(s => s.Id == id));

            // Act
            var result = await _service.GetNewestStoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Two valid stories
        }

        [Fact]
        public async Task SearchStoriesAsync_ReturnsEmpty_WhenNoMatch()
        {
            // Arrange
            var query = "Sports";
            var fakeStories = new List<Story>
            {
                new Story { Id = 1, Title = "Tech Advances in 2025", Url = "https://example.com/1" },
                new Story { Id = 2, Title = "Cooking Tips", Url = "https://example.com/2" }
            };

            _mockCacheManager
                .Setup(m => m.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Story>>>>()))
                .ReturnsAsync(fakeStories);

            // Act
            var result = await _service.SearchStoriesAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchStoriesAsync_ReturnsEmpty_WhenQueryIsNullOrEmpty()
        {
            // Act
            var resultNull = await _service.SearchStoriesAsync(null);
            var resultEmpty = await _service.SearchStoriesAsync(string.Empty);

            // Assert
            Assert.Empty(resultNull);
            Assert.Empty(resultEmpty);
        }

        [Fact]
        public async Task GetPagedStoriesAsync_ReturnsEmpty_ForInvalidPageOrPageSize()
        {
            // Arrange
            var fakeStories = Enumerable.Range(1, 50)
                .Select(i => new Story { Id = i, Title = $"Story {i}", Url = $"https://example.com/{i}" })
                .ToList();

            _mockCacheManager
                .Setup(m => m.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Story>>>>()))
                .ReturnsAsync(fakeStories);

            // Act
            var resultInvalidPage = await _service.GetPagedStoriesAsync(-1, 10);
            var resultInvalidPageSize = await _service.GetPagedStoriesAsync(1, -5);

            // Assert
            Assert.Empty(resultInvalidPage);
            Assert.Empty(resultInvalidPageSize);
        }

    }
}
