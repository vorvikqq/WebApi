using WebApi.Application.DTOs.Comment;
using WebApi.Application.Mappers;
using WebApi.Application.Repositories.Interfaces;
using WebApi.Application.Services.Interfaces;
using WebApi.Domain.Models;

namespace WebApi.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;

        public CommentService(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepository = commentRepo;
            _stockRepository = stockRepo;
        }
        public async Task<Comment> CreateCommentAsync(int stockId, CommentCreateRequest commentDto)
        {
            if (!await _stockRepository.IsExistAsnyc(stockId))
                throw new KeyNotFoundException("stock does not exist");

            var commentModel = commentDto.ToCommentFromCreateRequest(stockId);
            await _commentRepository.CreateAsync(commentModel);

            return commentModel;
        }

        public async Task DeleteCommentAsync(int id)
        {
            var deletedCount = await _commentRepository.DeleteAsync(id);

            if (deletedCount == 0)
                throw new KeyNotFoundException("Comment doesn't exist");
        }

        public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            var commentResponse = comments.Select(c => c.ToCommentResponse());

            return commentResponse;
        }

        public async Task<CommentResponseDto> GetCommentByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);

            if (comment == null)
                throw new KeyNotFoundException("Comment does not found");

            return comment.ToCommentResponse();
        }

        public async Task UpdateCommentAsync(int id, CommentUpdateRequest dto)
        {
            var updatedCount = await _commentRepository.UpdateAsync(id, dto);

            if (updatedCount == 0)
                throw new KeyNotFoundException("Comment doesn't exist");
        }
    }
}
