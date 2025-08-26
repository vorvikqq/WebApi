using WebApi.Infrastructure.Identity;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
