using WebApi.Application.DTOs.Account;
using WebApi.Infrastructure.Identity;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<AppUser> ValidateUserCredentialsAsync(string username, string password);
        UserResponse GetUserForResponse(AppUser user);
        Task<UserResponse> RegisterUserAsync(RegisterRequest registerDto);
    }
}
