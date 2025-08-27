using Microsoft.AspNetCore.Identity;
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

        public PortfolioService(UserManager<AppUser> userManager, IPortfolioRepository portfolioRepository)
        {
            _userManager = userManager;
            _portfolioRepository = portfolioRepository;
        }

        public async Task<List<Stock>> GetUserPortfolioAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            return await _portfolioRepository.GetUserPortfolioByUsername(username);
        }
    }
}
