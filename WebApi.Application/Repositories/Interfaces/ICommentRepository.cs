using WebApi.Application.DTOs.Comment;
using WebApi.Domain.Models;

namespace WebApi.Application.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> CreateAsync(Comment commentModel);
        Task<int> DeleteAsync(int id);
        Task<int> UpdateAsync(int id, CommentUpdateRequest dto);
    }
}
