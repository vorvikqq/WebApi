using WebApi.Domain.Models;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface IPortfolioService
    {
        Task<List<Stock>> GetUserPortfolioAsync(string username);
        Task AddPortfolio(string username, string stockSymbol);
        Task DeletePortfolio(string username, string stockSymbol);
    }
}
