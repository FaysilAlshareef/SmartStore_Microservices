using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartStore.IdentityServer.Models;

namespace SmartStore.IdentityServer.IdentityServerData
{
    public class IdentityServerDbContext:IdentityDbContext<ApplicationUser>
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options):base(options) 
        {
            
        }
    }
}
