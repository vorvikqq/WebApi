using WebApi.Application.DTOs.Account;
using WebApi.Infrastructure.Identity;

namespace WebApi.Infrastructure.Mapper
{
    public static class AppUserMapper
    {
        public static UserResponse ToUserResponse(this AppUser user, string token)
        {
            return new UserResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = token
            };
        }
    }
}
