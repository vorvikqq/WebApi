using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Models;
using WebApi.Infastructure.Data;
using WebApi.Infrastructure.Identity;

namespace WebApi.Infrastructure.Repositories
{
    public interface IPortfolioRepository
    {
        Task<List<Stock>> GetUserPortfolioByUsername(string username);
        Task AddStockToPortfolio(string username, Stock stock);
        Task DeleteStockFromPortfolio(string username, Stock stock);
    }


    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PortfolioRepository(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddStockToPortfolio(string username, Stock stock)
        {
            var userWithStocks = await _userManager.Users
                .Include(u => u.Stocks)
                .FirstOrDefaultAsync(u => u.UserName == username);

            userWithStocks?.Stocks.Add(stock);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockFromPortfolio(string username, Stock stock)
        {
            var userWithStocks = await _userManager.Users
                           .Include(u => u.Stocks)
                           .FirstOrDefaultAsync(u => u.UserName == username);

            userWithStocks?.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Stock>> GetUserPortfolioByUsername(string username)
        {
            var userWithStocks = await _userManager.Users
                .Include(u => u.Stocks)
                .ThenInclude(s => s.Comments)
                .FirstOrDefaultAsync(u => u.UserName == username);

            return userWithStocks?.Stocks.ToList() ?? new List<Stock>();
        }
    }
}
