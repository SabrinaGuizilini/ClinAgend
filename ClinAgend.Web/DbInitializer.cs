using Microsoft.AspNetCore.Identity;
using ClinAgend.Data.Models;
using ClinAgend.Models.Enums;

internal static class DbInitializer
{
    internal static async Task SeedUsers(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Name = "Usuário Admin",
            ClinicId = null,
            Type = UserType.SuperAdmin
        };

        var result = await userManager.CreateAsync(user, password);

    }
}

