using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Models;

namespace WebApi.Infrastructure.Data.Configurations
{
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> stockBuilder)
        {
            stockBuilder.HasKey(s => s.Id);

            stockBuilder.Property(s => s.Symbol).HasMaxLength(20);
            stockBuilder.Property(s => s.CompanyName).HasMaxLength(100);
            stockBuilder.Property(s => s.Purchase).HasColumnType("decimal(18,2)");
            stockBuilder.Property(s => s.LastDiv).HasColumnType("decimal(18,2)");
            stockBuilder.Property(s => s.Industry).HasMaxLength(100);

            stockBuilder
                .HasMany(s => s.Comments)
                .WithOne(c => c.Stock)
                .HasForeignKey(c => c.StockId);
        }
    }
}
