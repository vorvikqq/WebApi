using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.DTOs.Comment;
using WebApi.Domain.Models;
using WebApi.Infastructure.Data;
using WebApi.Infastructure.Repositories;

namespace WebApi.Infrastructure.Tests
{
    public class CommentRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests()
        {
            // Create in-memory SQLite database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _repository = new CommentRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        private async Task<Stock> SeedStock(string symbol = "AAPL", string companyName = "Apple Inc.")
        {
            var stock = new Stock
            {
                Symbol = symbol,
                CompanyName = companyName,
                Purchase = 150.00m,
                LastDiv = 2.50m,
                Industry = "Technology",
                MarketCap = 2500000000000
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        private async Task<List<Comment>> SeedComments(int? stockId = null)
        {
            var stock = stockId.HasValue ? await _context.Stocks.FindAsync(stockId.Value) : await SeedStock();

            var comments = new List<Comment>
        {
            new Comment
            {
                Title = "Great stock!",
                Content = "This is a solid investment choice.",
                CreatedOn = DateTime.Now.AddDays(-2),
                StockId = stock!.Id
            },
            new Comment
            {
                Title = "Overvalued",
                Content = "I think this stock is currently overpriced.",
                CreatedOn = DateTime.Now.AddDays(-1),
                StockId = stock.Id
            }
        };

            _context.Comments.AddRange(comments);
            await _context.SaveChangesAsync();
            return comments;
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WhenCommentsExist_ShouldReturnAllComments()
        {
            // Arrange
            var seededComments = await SeedComments();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(seededComments, options => options
                .Excluding(c => c.Stock)); // Exclude navigation property for comparison
        }

        [Fact]
        public async Task GetAllAsync_WhenNoCommentsExist_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ShouldUseAsNoTracking()
        {
            // Arrange
            await SeedComments();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();

            // Verify that entities are not tracked
            foreach (var comment in result)
            {
                var entry = _context.Entry(comment);
                entry.State.Should().Be(EntityState.Detached);
            }
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenCommentExists_ShouldReturnComment()
        {
            // Arrange
            var seededComments = await SeedComments();
            var targetComment = seededComments.First();

            // Act
            var result = await _repository.GetByIdAsync(targetComment.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(targetComment.Id);
            result.Title.Should().Be(targetComment.Title);
            result.Content.Should().Be(targetComment.Content);
            result.StockId.Should().Be(targetComment.StockId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCommentDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WhenValidComment_ShouldCreateAndReturnComment()
        {
            // Arrange
            var stock = await SeedStock();
            var newComment = new Comment
            {
                Title = "New Comment",
                Content = "This is a new comment content.",
                StockId = stock.Id,
                CreatedOn = DateTime.Now
            };

            // Act
            var result = await _repository.CreateAsync(newComment);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0); // Should have generated ID
            result.Title.Should().Be("New Comment");
            result.Content.Should().Be("This is a new comment content.");
            result.StockId.Should().Be(stock.Id);

            // Verify it's actually saved in database
            var savedComment = await _context.Comments.FindAsync(result.Id);
            savedComment.Should().NotBeNull();
            savedComment.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task CreateAsync_WhenCommentWithoutStockId_ShouldCreateSuccessfully()
        {
            // Arrange
            var newComment = new Comment
            {
                Title = "Standalone Comment",
                Content = "This comment has no stock association.",
                StockId = null,
                CreatedOn = DateTime.Now
            };

            // Act
            var result = await _repository.CreateAsync(newComment);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.StockId.Should().BeNull();

            // Verify in database
            var savedComment = await _context.Comments.FindAsync(result.Id);
            savedComment.Should().NotBeNull();
            savedComment!.StockId.Should().BeNull();
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenCommentExists_ShouldDeleteAndReturnCount()
        {
            // Arrange
            var seededComments = await SeedComments();
            var commentToDelete = seededComments.First();

            // Act
            var result = await _repository.DeleteAsync(commentToDelete.Id);

            // Assert
            result.Should().Be(1); // Should return number of affected rows

            // Verify comment is deleted
            var deletedComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentToDelete.Id);
            deletedComment.Should().BeNull();

            // Verify other comments still exist
            var remainingComments = await _context.Comments.ToListAsync();
            remainingComments.Should().HaveCount(1);
        }

        [Fact]
        public async Task DeleteAsync_WhenCommentDoesNotExist_ShouldReturnZero()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            result.Should().Be(0); // No rows affected
        }

        [Fact]
        public async Task DeleteAsync_WhenMultipleCommentsExist_ShouldOnlyDeleteSpecifiedComment()
        {
            // Arrange
            var seededComments = await SeedComments();
            var commentToDelete = seededComments.First();
            var commentToKeep = seededComments.Last();

            // Act
            var result = await _repository.DeleteAsync(commentToDelete.Id);

            // Assert
            result.Should().Be(1);

            // Verify correct comment is deleted
            var deletedComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentToDelete.Id);
            deletedComment.Should().BeNull();

            // Verify other comment still exists
            var remainingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentToKeep.Id);
            remainingComment.Should().NotBeNull();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WhenCommentExists_ShouldUpdateAndReturnCount()
        {
            // Arrange
            var seededComments = await SeedComments();
            var commentToUpdate = seededComments.First();
            var updateDto = new CommentUpdateRequest
            {
                Title = "Updated Title",
                Content = "Updated content for the comment."
            };

            // Act
            var result = await _repository.UpdateAsync(commentToUpdate.Id, updateDto);

            // Assert
            result.Should().Be(1); // Should return number of affected rows

            // Verify comment is updated in database
            var updatedComment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentToUpdate.Id);
            updatedComment.Should().NotBeNull();
            updatedComment!.Title.Should().Be("Updated Title");
            updatedComment.Content.Should().Be("Updated content for the comment.");

            // Verify other properties remain unchanged
            updatedComment.Id.Should().Be(commentToUpdate.Id);
            updatedComment.StockId.Should().Be(commentToUpdate.StockId);
            updatedComment.CreatedOn.Should().BeCloseTo(commentToUpdate.CreatedOn, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task UpdateAsync_WhenCommentDoesNotExist_ShouldReturnZero()
        {
            // Arrange
            var updateDto = new CommentUpdateRequest
            {
                Title = "Non-existent Comment",
                Content = "This update should not affect any records."
            };

            // Act
            var result = await _repository.UpdateAsync(999, updateDto);

            // Assert
            result.Should().Be(0); // No rows affected
        }

        [Fact]
        public async Task UpdateAsync_WhenMultipleCommentsExist_ShouldOnlyUpdateSpecifiedComment()
        {
            // Arrange
            var seededComments = await SeedComments();
            var commentToUpdate = seededComments.First();
            var commentToKeepUnchanged = seededComments.Last();
            var originalTitle = commentToKeepUnchanged.Title;
            var originalContent = commentToKeepUnchanged.Content;

            var updateDto = new CommentUpdateRequest
            {
                Title = "Updated Title",
                Content = "Updated content."
            };

            // Act
            var result = await _repository.UpdateAsync(commentToUpdate.Id, updateDto);

            // Assert
            result.Should().Be(1);

            // Verify correct comment is updated
            var updatedComment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentToUpdate.Id);
            updatedComment!.Title.Should().Be("Updated Title");
            updatedComment.Content.Should().Be("Updated content.");

            // Verify other comment remains unchanged
            var unchangedComment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentToKeepUnchanged.Id);
            unchangedComment!.Title.Should().Be(originalTitle);
            unchangedComment.Content.Should().Be(originalContent);
        }

        #endregion
    }
}
