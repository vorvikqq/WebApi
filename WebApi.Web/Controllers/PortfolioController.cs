using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Services.Interfaces;
using WebApi.Infrastructure.Identity;
using WebApi.Web.Extensions;

namespace WebApi.Web.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockService _stockService;

        public PortfolioController(UserManager<AppUser> userManager, IStockService stockService)
        {
            _userManager = userManager;
            _stockService = stockService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);



            return Ok();
        }
    }
}
