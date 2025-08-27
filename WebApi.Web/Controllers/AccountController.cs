using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs.Account;
using WebApi.Infrastructure.Identity;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Web.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AppUser user;

            try
            {
                user = await _accountService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }

            var userResponse = _accountService.GetUserForResponse(user);

            return Ok(userResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userResponse = await _accountService.RegisterUserAsync(registerDto);
                return Ok(userResponse);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred during registration");
            }
        }
    }
}
