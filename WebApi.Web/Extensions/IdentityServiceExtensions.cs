using Microsoft.AspNetCore.Identity;
using WebApi.Infastructure.Data;
using WebApi.Infrastructure.Identity;

namespace WebApi.Web.Identity
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}
