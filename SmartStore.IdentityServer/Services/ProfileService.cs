using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using SmartStore.IdentityServer.Models;
using System.Security.Claims;

namespace SmartStore.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
           
            if (user != null)
            {
                List<Claim> claims = new();
                claims = claims.Where(cliam => context.RequestedClaimTypes
                             .Contains(cliam.Type)).ToList();

                claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));
                claims.Add(new Claim(JwtClaimTypes.FamilyName , user.LastName));
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
          
            
                IList<string> roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));

                }
                context.IssuedClaims = claims;
            }
           
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);

            context.IsActive= user != null;
        }
    }
}
