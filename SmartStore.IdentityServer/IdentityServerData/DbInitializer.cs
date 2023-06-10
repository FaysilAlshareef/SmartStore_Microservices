using IdentityModel;
using Microsoft.AspNetCore.Identity;
using SmartStore.IdentityServer.Models;
using System.Security.Claims;

namespace SmartStore.IdentityServer.IdentityServerData
{
    public static class DbInitializer
    {
        public static async void Initialize(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                await roleManager.CreateAsync(new IdentityRole(SD.Admin));
                await roleManager.CreateAsync(new IdentityRole(SD.Customer));
            }
            else { return; }

            ApplicationUser adminUser = new()
            {
                UserName = "admin@matgr.com",
                Email = "admin@matgr.com",
                EmailConfirmed = true,
                PhoneNumber = "01004207522",
                FirstName = "Muhammad",
                LastName = "Awadallah"
            };

            await userManager.CreateAsync(adminUser, "Password1!");
            await userManager.AddToRoleAsync(adminUser, SD.Admin);

            await userManager.AddClaimsAsync(adminUser, new Claim[] {
                new Claim(JwtClaimTypes.Name,adminUser.FirstName+" "+ adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName,adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName,adminUser.LastName),
                new Claim(JwtClaimTypes.Role,SD.Admin),
            });

            ApplicationUser customerUser = new ApplicationUser()
            {
                UserName = "amir@matgr.com",
                Email = "amir@matgr.com",
                EmailConfirmed = true,
                PhoneNumber = "01004207522",
                FirstName = "Amir",
                LastName = "Muhammad"
            };

            await userManager.CreateAsync(customerUser, "Password1!");
            await userManager.AddToRoleAsync(customerUser, SD.Customer);

            await userManager.AddClaimsAsync(customerUser, new Claim[] {
                new Claim(JwtClaimTypes.Name,customerUser.FirstName+" "+ customerUser.LastName),
                new Claim(JwtClaimTypes.GivenName,customerUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName,customerUser.LastName),
                new Claim(JwtClaimTypes.Role,SD.Customer),
            });
        }
    }
}
