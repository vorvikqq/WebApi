using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs.Account;
using WebApi.Infrastructure.Identity;
using WebApi.Infrastructure.Mapper;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
        {
            //add validation username and password is null or empty
            AppUser user;

            try
            {
                user = await _accountService.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }

            var userResponse = user.ToUserResponse(_tokenService.CreateToken(user));

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
            catch (Exception)
            {
                return StatusCode(500, "An error occurred during registration");
            }
        }
    }
}
