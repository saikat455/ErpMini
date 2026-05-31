// ErpMini.Infrastructure/Data/DbSeeder.cs
using ErpMini.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ErpMini.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // ── Seed Roles ─────────────────────────────────────────────
        string[] roles = { "Admin", "HR", "Finance", "Employee" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ── Seed Admin User ────────────────────────────────────────
        const string adminEmail    = "admin@erpmini.com";
        const string adminPassword = "Admin@1234";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                FullName       = "System Admin",
                UserName       = adminEmail,
                Email          = adminEmail,
                EmailConfirmed = true,
                IsActive       = true,
                CreatedAt      = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // ── Seed HR User ───────────────────────────────────────────
        const string hrEmail    = "hr@erpmini.com";
        const string hrPassword = "Hr@12345";

        var hrUser = await userManager.FindByEmailAsync(hrEmail);

        if (hrUser is null)
        {
            hrUser = new ApplicationUser
            {
                FullName       = "HR Manager",
                UserName       = hrEmail,
                Email          = hrEmail,
                EmailConfirmed = true,
                IsActive       = true,
                CreatedAt      = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(hrUser, hrPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(hrUser, "HR");
        }
    }
}