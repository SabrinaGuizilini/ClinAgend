using System.Security.Claims;
using ClinAgend.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ClinAgend.Web;

public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public AppClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim("FullName", user.Name));

        if (user.ClinicId.HasValue)
        {
            identity.AddClaim(
                new Claim("ClinicId", user.ClinicId.Value.ToString())
            );
        }

        identity.AddClaim(new Claim("UserType", user.Type.ToString()));

        return identity;
    }
}
