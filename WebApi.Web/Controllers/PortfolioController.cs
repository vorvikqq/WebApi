using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var username = User.GetUsername();

            var userStocks = await _portfolioService.GetUserPortfolioAsync(username);

            return Ok(userStocks);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();

            try
            {
                await _portfolioService.AddPortfolio(username, symbol);
            }
            catch (Exception e)
            {
                //Краще кожну окремо обробити помилку. Ну це на майбутнє
                return BadRequest(e.Message);
            }

            return Ok("added");
        }
    }
}
