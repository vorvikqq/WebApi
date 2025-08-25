using Microsoft.EntityFrameworkCore;
using WebApi.Application.DTOs.Comment;
using WebApi.Application.Repositories.Interfaces;
using WebApi.Domain.Models;
using WebApi.Infastructure.Data;

namespace WebApi.Infastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.AsNoTracking().ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();

            return commentModel;
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _context.Comments
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<int> UpdateAsync(int id, CommentUpdateRequest dto)
        {
            return await _context.Comments
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.Title, dto.Title)
                    .SetProperty(c => c.Content, dto.Content)
                );
        }
    }
}
