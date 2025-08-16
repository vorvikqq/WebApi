using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Comment> Comment { get; set; }

    }
}
