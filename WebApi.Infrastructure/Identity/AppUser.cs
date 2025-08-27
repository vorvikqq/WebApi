using Microsoft.AspNetCore.Identity;
using WebApi.Domain.Models;

namespace WebApi.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
