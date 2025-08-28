using WebApi.Application.DTOs.Comment;
using WebApi.Domain.Models;

namespace WebApi.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync();
        Task<CommentResponseDto?> GetCommentByIdAsync(int id);
        Task<Comment?> CreateCommentAsync(int stockId, CommentCreateRequest commentModel);
        Task<bool> DeleteCommentAsync(int id);
        Task<bool> UpdateCommentAsync(int id, CommentUpdateRequest dto);
    }
}
