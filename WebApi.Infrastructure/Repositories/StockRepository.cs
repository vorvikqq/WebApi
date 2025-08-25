using Microsoft.EntityFrameworkCore;
using WebApi.Application.DTOs.Stock;
using WebApi.Application.Repositories.Interfaces;
using WebApi.Domain.Models;
using WebApi.Infastructure.Data;

namespace WebApi.Infastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;
        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Stock>> GetAllAsync()
        {
            return await _context.Stocks
                .Include(s => s.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks
                .Include(s => s.Comments)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();

            return stock;
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _context.Stocks
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<int> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            return await _context.Stocks
                .Where(s => s.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(s => s.Symbol, stockDto.Symbol)
                    .SetProperty(s => s.CompanyName, stockDto.CompanyName)
                    .SetProperty(s => s.Purchase, stockDto.Purchase)
                    .SetProperty(s => s.LastDiv, stockDto.LastDiv)
                    .SetProperty(s => s.Industry, stockDto.Industry)
                    .SetProperty(s => s.MarketCap, stockDto.MarketCap)
                    );
        }

        public async Task<bool> IsExistAsnyc(int id)
        {
            return await _context.Stocks.AnyAsync(c => c.Id == id);
        }
    }
}
