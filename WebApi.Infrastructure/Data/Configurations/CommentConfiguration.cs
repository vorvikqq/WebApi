using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Models;

namespace WebApi.Infrastructure.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> commentBuilder)
        {
            commentBuilder
                .HasKey(c => c.Id);

            commentBuilder.Property(c => c.Title).HasMaxLength(50);
            commentBuilder.Property(c => c.Content).HasMaxLength(150);

            commentBuilder
                .HasOne(c => c.Stock)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.StockId);
        }
    }
}
