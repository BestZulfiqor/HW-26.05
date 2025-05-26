using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Domain.DTOs.Seeder;

public class DefaultUser
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = "theassassinaa@gmail.com",
            Email = "theassassinaa@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "205221144"
        };

        var existingUser = await userManager.FindByNameAsync(user.UserName);
        if (existingUser != null)
        {
            return;
        }

        var result = await userManager.CreateAsync(user, "9000");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}