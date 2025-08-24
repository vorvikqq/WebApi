using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs.Stock;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
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
            return await _context.Stocks.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.FindAsync(id);
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
    }
}
