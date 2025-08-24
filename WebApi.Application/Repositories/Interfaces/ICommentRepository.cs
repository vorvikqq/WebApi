using WebApi.Domain.Models;

namespace WebApi.Application.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllAsync();
    }
}
