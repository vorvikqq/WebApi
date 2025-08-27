using System.Security.Claims;

namespace WebApi.Web.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value
                ?? throw new InvalidOperationException("Username not found");
        }
    }
}
