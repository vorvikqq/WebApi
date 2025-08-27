using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Services.Interfaces;
using WebApi.Web.Extensions;

namespace WebApi.Web.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var username = User.GetUsername();

            var userStocks = await _portfolioService.GetUserPortfolioAsync(username);

            return Ok(userStocks);
        }
    }
}
