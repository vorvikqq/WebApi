using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.DTOs.Account;
using WebApi.Infrastructure.Identity;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AppUser> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            var validPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!validPassword)
                throw new UnauthorizedAccessException("Invalid username or password");

            return user;
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username.ToLower());

            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password");

            return user;
        }

        public UserResponse GetUserForResponse(AppUser user)
        {
            return new UserResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<UserResponse> RegisterUserAsync(RegisterRequest registerDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == registerDto.Username.ToLower());

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this username or email already exists");
            }

            var appUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!createdUser.Succeeded)
                throw new InvalidOperationException("User creation failed.");

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

            if (!roleResult.Succeeded)
                throw new InvalidOperationException("Role assignment failed.");

            return GetUserForResponse(appUser);
        }
    }
}
