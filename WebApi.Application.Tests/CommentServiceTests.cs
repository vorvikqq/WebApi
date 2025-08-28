using FluentAssertions;
using Moq;
using WebApi.Application.DTOs.Comment;
using WebApi.Application.Repositories.Interfaces;
using WebApi.Application.Services;
using WebApi.Domain.Models;

namespace WebApi.Application.Tests
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<IStockRepository> _mockStockRepository;
        private readonly CommentService _commentService;

        public CommentServiceTests()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockStockRepository = new Mock<IStockRepository>();
            _commentService = new CommentService(_mockCommentRepository.Object, _mockStockRepository.Object);
        }

        #region CreateCommentAsync Tests

        [Fact]
        public async Task CreateCommentAsync_WhenStockExists_ShouldReturnCreatedComment()
        {
            // Arrange
            var stockId = 1;
            var commentDto = new CommentCreateRequest { Content = "Test comment", Title = "Test title" };
            var expectedComment = new Comment { Id = 1, StockId = stockId, Content = "Test comment", Title = "Test title", CreatedOn = DateTime.Now };

            _mockStockRepository.Setup(x => x.IsExistAsnyc(stockId)).ReturnsAsync(true);
            _mockCommentRepository.Setup(x => x.CreateAsync(It.IsAny<Comment>())).ReturnsAsync(expectedComment);

            // Act
            var result = await _commentService.CreateCommentAsync(stockId, commentDto);

            // Assert
            result.Should().NotBeNull();
            result!.StockId.Should().Be(stockId);
            result.Content.Should().Be(commentDto.Content);
            result.Title.Should().Be(commentDto.Title);
            result.CreatedOn.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5)); // Allow small time difference

            _mockStockRepository.Verify(x => x.IsExistAsnyc(stockId), Times.Once);
            _mockCommentRepository.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task CreateCommentAsync_WhenStockDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var stockId = 999;
            var commentDto = new CommentCreateRequest { Content = "Test comment", Title = "Test title" };

            _mockStockRepository.Setup(x => x.IsExistAsnyc(stockId)).ReturnsAsync(false);

            // Act
            var result = await _commentService.CreateCommentAsync(stockId, commentDto);

            // Assert
            result.Should().BeNull();

            _mockStockRepository.Verify(x => x.IsExistAsnyc(stockId), Times.Once);
            _mockCommentRepository.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Never);
        }

        #endregion

        #region DeleteCommentAsync Tests

        [Fact]
        public async Task DeleteCommentAsync_WhenCommentDeleted_ShouldReturnTrue()
        {
            // Arrange
            var commentId = 1;
            var deletedCount = 1;

            _mockCommentRepository.Setup(x => x.DeleteAsync(commentId)).ReturnsAsync(deletedCount);

            // Act
            var result = await _commentService.DeleteCommentAsync(commentId);

            // Assert
            result.Should().BeTrue();
            _mockCommentRepository.Verify(x => x.DeleteAsync(commentId), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_WhenNoCommentDeleted_ShouldReturnFalse()
        {
            // Arrange
            var commentId = 999;
            var deletedCount = 0;

            _mockCommentRepository.Setup(x => x.DeleteAsync(commentId)).ReturnsAsync(deletedCount);

            // Act
            var result = await _commentService.DeleteCommentAsync(commentId);

            // Assert
            result.Should().BeFalse();
            _mockCommentRepository.Verify(x => x.DeleteAsync(commentId), Times.Once);
        }

        #endregion

        #region GetAllCommentsAsync Tests

        [Fact]
        public async Task GetAllCommentsAsync_WhenCommentsExist_ShouldReturnCommentResponseDtos()
        {
            // Arrange
            var createdDate1 = DateTime.Now.AddDays(-1);
            var createdDate2 = DateTime.Now.AddDays(-2);

            var comments = new List<Comment>
        {
            new Comment { Id = 1, StockId = 1, Content = "Comment 1", Title = "Title 1", CreatedOn = createdDate1 },
            new Comment { Id = 2, StockId = 2, Content = "Comment 2", Title = "Title 2", CreatedOn = createdDate2 }
        };

            _mockCommentRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(comments);

            // Act
            var result = await _commentService.GetAllCommentsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var resultList = result.ToList();
            resultList[0].Id.Should().Be(1);
            resultList[0].StockId.Should().Be(1);
            resultList[0].Content.Should().Be("Comment 1");
            resultList[0].Title.Should().Be("Title 1");
            resultList[0].CreatedOn.Should().Be(createdDate1);

            resultList[1].Id.Should().Be(2);
            resultList[1].StockId.Should().Be(2);
            resultList[1].Content.Should().Be("Comment 2");
            resultList[1].Title.Should().Be("Title 2");
            resultList[1].CreatedOn.Should().Be(createdDate2);

            _mockCommentRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCommentsAsync_WhenNoCommentsExist_ShouldReturnEmptyCollection()
        {
            // Arrange
            var emptyComments = new List<Comment>();

            _mockCommentRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyComments);

            // Act
            var result = await _commentService.GetAllCommentsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockCommentRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        #endregion

        #region GetCommentByIdAsync Tests

        [Fact]
        public async Task GetCommentByIdAsync_WhenCommentExists_ShouldReturnCommentResponseDto()
        {
            // Arrange
            var commentId = 1;
            var createdDate = DateTime.Now.AddDays(-1);
            var comment = new Comment { Id = commentId, StockId = 1, Content = "Test comment", Title = "Test title", CreatedOn = createdDate };

            _mockCommentRepository.Setup(x => x.GetByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            var result = await _commentService.GetCommentByIdAsync(commentId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(commentId);
            result.StockId.Should().Be(1);
            result.Content.Should().Be("Test comment");
            result.Title.Should().Be("Test title");
            result.CreatedOn.Should().Be(createdDate);

            _mockCommentRepository.Verify(x => x.GetByIdAsync(commentId), Times.Once);
        }

        [Fact]
        public async Task GetCommentByIdAsync_WhenCommentDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var commentId = 999;
            Comment? nullComment = null;

            _mockCommentRepository.Setup(x => x.GetByIdAsync(commentId)).ReturnsAsync(nullComment);

            // Act
            var result = await _commentService.GetCommentByIdAsync(commentId);

            // Assert
            result.Should().BeNull();

            _mockCommentRepository.Verify(x => x.GetByIdAsync(commentId), Times.Once);
        }

        #endregion

        #region UpdateCommentAsync Tests

        [Fact]
        public async Task UpdateCommentAsync_WhenCommentUpdated_ShouldReturnTrue()
        {
            // Arrange
            var commentId = 1;
            var updateDto = new CommentUpdateRequest { Content = "Updated content", Title = "Updated title" };
            var updatedCount = 1;

            _mockCommentRepository.Setup(x => x.UpdateAsync(commentId, updateDto)).ReturnsAsync(updatedCount);

            // Act
            var result = await _commentService.UpdateCommentAsync(commentId, updateDto);

            // Assert
            result.Should().BeTrue();
            _mockCommentRepository.Verify(x => x.UpdateAsync(commentId, updateDto), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_WhenNoCommentUpdated_ShouldReturnFalse()
        {
            // Arrange
            var commentId = 999;
            var updateDto = new CommentUpdateRequest { Content = "Updated content", Title = "Updated title" };
            var updatedCount = 0;

            _mockCommentRepository.Setup(x => x.UpdateAsync(commentId, updateDto)).ReturnsAsync(updatedCount);

            // Act
            var result = await _commentService.UpdateCommentAsync(commentId, updateDto);

            // Assert
            result.Should().BeFalse();
            _mockCommentRepository.Verify(x => x.UpdateAsync(commentId, updateDto), Times.Once);
        }

        #endregion
    }
}
