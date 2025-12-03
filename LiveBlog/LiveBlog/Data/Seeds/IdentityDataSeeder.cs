using LiveBlog.Models.IdentityUser;
using Microsoft.AspNetCore.Identity;

namespace LiveBlog.Data.Seeds;

public static class IdentityDataSeeder
{
    public static async Task SeedDataAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<MyIdentityUserEntity>>();

        string[] roleNames = { "Admin", "User" };

        // === Создаём роли ===
        foreach (var roleName in roleNames)
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"✅ Role created: {roleName}");
            }

        // === Создаём администратора ===
        var adminEmail = "admin@admin.com";
        var adminPassword = "Admin123!"; // ❗️ можно вынести в конфиг

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new MyIdentityUserEntity()
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"✅ Admin user created: {adminEmail}");
            }
            else
            {
                Console.WriteLine($"⚠️ Failed to create admin user: {string.Join(", ", result.Errors)}");
            }
        }
        else
        {
            // гарантируем, что он в роли Admin
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}