using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Application.DTOs.Comment;
using WebApi.Application.Services.Interfaces;
using WebApi.Domain.Models;
using WebApi.Web.Controllers;

namespace WebApi.Web.Tests
{
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _mockCommentService;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            _mockCommentService = new Mock<ICommentService>();
            _controller = new CommentController(_mockCommentService.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnOkWithComments()
        {
            // Arrange
            var comments = new List<CommentResponseDto>
        {
            new CommentResponseDto { Id = 1, Title = "Comment 1", Content = "Content 1", StockId = 1 },
            new CommentResponseDto { Id = 2, Title = "Comment 2", Content = "Content 2", StockId = 2 }
        };

            _mockCommentService.Setup(x => x.GetAllCommentsAsync()).ReturnsAsync(comments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedComments = okResult.Value.Should().BeAssignableTo<IEnumerable<CommentResponseDto>>().Subject;
            returnedComments.Should().HaveCount(2);
            returnedComments.Should().BeEquivalentTo(comments);

            _mockCommentService.Verify(x => x.GetAllCommentsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoComments_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            var emptyComments = new List<CommentResponseDto>();
            _mockCommentService.Setup(x => x.GetAllCommentsAsync()).ReturnsAsync(emptyComments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedComments = okResult.Value.Should().BeAssignableTo<IEnumerable<CommentResponseDto>>().Subject;
            returnedComments.Should().BeEmpty();

            _mockCommentService.Verify(x => x.GetAllCommentsAsync(), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WhenCommentExists_ShouldReturnOkWithComment()
        {
            // Arrange
            var commentId = 1;
            var comment = new CommentResponseDto { Id = commentId, Title = "Test Comment", Content = "Test Content", StockId = 1 };

            _mockCommentService.Setup(x => x.GetCommentByIdAsync(commentId)).ReturnsAsync(comment);

            // Act
            var result = await _controller.GetById(commentId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedComment = okResult.Value.Should().BeOfType<CommentResponseDto>().Subject;
            returnedComment.Should().BeEquivalentTo(comment);

            _mockCommentService.Verify(x => x.GetCommentByIdAsync(commentId), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCommentDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var commentId = 999;
            _mockCommentService.Setup(x => x.GetCommentByIdAsync(commentId)).ReturnsAsync((CommentResponseDto?)null);

            // Act
            var result = await _controller.GetById(commentId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();

            _mockCommentService.Verify(x => x.GetCommentByIdAsync(commentId), Times.Once);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WhenValidRequest_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var stockId = 1;
            var createRequest = new CommentCreateRequest { Title = "New Comment", Content = "New Content" };
            var createdComment = new Comment
            {
                Id = 1,
                StockId = stockId,
                Title = "New Comment",
                Content = "New Content",
                CreatedOn = DateTime.Now
            };

            _mockCommentService.Setup(x => x.CreateCommentAsync(stockId, createRequest)).ReturnsAsync(createdComment);

            // Act
            var result = await _controller.Create(stockId, createRequest);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdComment.Id);

            var returnedComment = createdResult.Value.Should().BeOfType<CommentResponseDto>().Subject;
            returnedComment.Id.Should().Be(createdComment.Id);
            returnedComment.Title.Should().Be(createdComment.Title);
            returnedComment.Content.Should().Be(createdComment.Content);
            returnedComment.StockId.Should().Be(createdComment.StockId);

            _mockCommentService.Verify(x => x.CreateCommentAsync(stockId, createRequest), Times.Once);
        }

        [Fact]
        public async Task Create_WhenStockNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var stockId = 999;
            var createRequest = new CommentCreateRequest { Title = "New Comment", Content = "New Content" };

            _mockCommentService.Setup(x => x.CreateCommentAsync(stockId, createRequest)).ReturnsAsync((Comment?)null);

            // Act
            var result = await _controller.Create(stockId, createRequest);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("stock not found");

            _mockCommentService.Verify(x => x.CreateCommentAsync(stockId, createRequest), Times.Once);
        }

        [Fact]
        public async Task Create_WhenTitleIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var stockId = 1;
            var createRequest = new CommentCreateRequest { Title = "", Content = "Valid content" };

            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.Create(stockId, createRequest);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeOfType<SerializableError>();

            _mockCommentService.Verify(x => x.CreateCommentAsync(It.IsAny<int>(), It.IsAny<CommentCreateRequest>()), Times.Never);
        }

        [Fact]
        public async Task Create_WhenTitleExceedsMaxLength_ShouldReturnBadRequest()
        {
            // Arrange
            var stockId = 1;
            var longTitle = new string('a', 51); // 51 characters, exceeds 50 char limit
            var createRequest = new CommentCreateRequest { Title = longTitle, Content = "Valid content" };

            _controller.ModelState.AddModelError("Title", "Title can not be over 50 characters");

            // Act
            var result = await _controller.Create(stockId, createRequest);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeOfType<SerializableError>();

            _mockCommentService.Verify(x => x.CreateCommentAsync(It.IsAny<int>(), It.IsAny<CommentCreateRequest>()), Times.Never);
        }

        [Fact]
        public async Task Create_WhenContentExceedsMaxLength_ShouldReturnBadRequest()
        {
            // Arrange
            var stockId = 1;
            var longContent = new string('b', 151); // 151 characters, exceeds 150 char limit
            var createRequest = new CommentCreateRequest { Title = "Valid title", Content = longContent };

            _controller.ModelState.AddModelError("Content", "Content can not be over 150 characters");

            // Act
            var result = await _controller.Create(stockId, createRequest);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeOfType<SerializableError>();

            _mockCommentService.Verify(x => x.CreateCommentAsync(It.IsAny<int>(), It.IsAny<CommentCreateRequest>()), Times.Never);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WhenValidRequest_ShouldReturnOkWithUpdatedDto()
        {
            // Arrange
            var commentId = 1;
            var updateRequest = new CommentUpdateRequest { Title = "Updated Title", Content = "Updated Content" };

            _mockCommentService.Setup(x => x.UpdateCommentAsync(commentId, updateRequest)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(commentId, updateRequest);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDto = okResult.Value.Should().BeOfType<CommentUpdateRequest>().Subject;
            returnedDto.Should().BeEquivalentTo(updateRequest);

            _mockCommentService.Verify(x => x.UpdateCommentAsync(commentId, updateRequest), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCommentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var commentId = 999;
            var updateRequest = new CommentUpdateRequest { Title = "Updated Title", Content = "Updated Content" };

            _mockCommentService.Setup(x => x.UpdateCommentAsync(commentId, updateRequest)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(commentId, updateRequest);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Comment not found");

            _mockCommentService.Verify(x => x.UpdateCommentAsync(commentId, updateRequest), Times.Once);
        }

        [Fact]
        public async Task Update_WhenTitleIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var commentId = 1;
            var updateRequest = new CommentUpdateRequest { Title = "", Content = "Valid content" };

            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.Update(commentId, updateRequest);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeOfType<SerializableError>();

            _mockCommentService.Verify(x => x.UpdateCommentAsync(It.IsAny<int>(), It.IsAny<CommentUpdateRequest>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenTitleExceedsMaxLength_ShouldReturnBadRequest()
        {
            // Arrange
            var commentId = 1;
            var longTitle = new string('a', 51); // 51 characters, exceeds 50 char limit
            var updateRequest = new CommentUpdateRequest { Title = longTitle, Content = "Valid content" };

            _controller.ModelState.AddModelError("Title", "Title can not be over 50 characters");

            // Act
            var result = await _controller.Update(commentId, updateRequest);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeOfType<SerializableError>();

            _mockCommentService.Verify(x => x.UpdateCommentAsync(It.IsAny<int>(), It.IsAny<CommentUpdateRequest>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenContentExceedsMaxLength_ShouldReturnBadRequest()
        {
            // Arrange
            var commentId = 1;
            var longContent = new string('b', 151); // 151 characters, exceeds 150 char limit
            var updateRequest = new CommentUpdateRequest { Title = "Valid title", Content = longContent };

            _controller.ModelState.AddModelError("Content", "Content can not be over 150 characters");

            // Act
            var result = await _controller.Update(commentId, updateRequest);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeOfType<SerializableError>();

            _mockCommentService.Verify(x => x.UpdateCommentAsync(It.IsAny<int>(), It.IsAny<CommentUpdateRequest>()), Times.Never);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WhenCommentExists_ShouldReturnNoContent()
        {
            // Arrange
            var commentId = 1;
            _mockCommentService.Setup(x => x.DeleteCommentAsync(commentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _mockCommentService.Verify(x => x.DeleteCommentAsync(commentId), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenCommentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var commentId = 999;
            _mockCommentService.Setup(x => x.DeleteCommentAsync(commentId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Comment not found");

            _mockCommentService.Verify(x => x.DeleteCommentAsync(commentId), Times.Once);
        }

        #endregion
    }
}
