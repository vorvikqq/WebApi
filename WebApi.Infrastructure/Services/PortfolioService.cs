using Microsoft.AspNetCore.Identity;
using WebApi.Application.Repositories.Interfaces;
using WebApi.Domain.Models;
using WebApi.Infrastructure.Identity;
using WebApi.Infrastructure.Repositories;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Infrastructure.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IStockRepository _stockRepository;

        public PortfolioService(UserManager<AppUser> userManager, IPortfolioRepository portfolioRepository, IStockRepository stockRepository)
        {
            _userManager = userManager;
            _portfolioRepository = portfolioRepository;
            _stockRepository = stockRepository;
        }

        public async Task<List<Stock>> GetUserPortfolioAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            return await _portfolioRepository.GetUserPortfolioByUsername(username);
        }

        public async Task AddPortfolio(string username, string stockSymbol)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            var stock = await _stockRepository.GetBySymbolAsync(stockSymbol);

            if (stock == null)
                throw new KeyNotFoundException("Stock not found");

            var userPortfolio = await GetUserPortfolioAsync(username);

            if (userPortfolio.Any(s => s.Symbol.ToLower() == stockSymbol.ToLower()))
                throw new InvalidOperationException("Cannot add same stock to portfolio");

            await _portfolioRepository.AddStockToPortfolio(username, stock);
        }
    }
}
