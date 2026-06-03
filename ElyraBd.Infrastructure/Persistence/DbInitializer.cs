using ElyraBd.Core.Constants;
using ElyraBd.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElyraBd.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Migration failed; attempting EnsureCreated for development.");
            await context.Database.EnsureCreatedAsync();
        }

        await SeedRolesAsync(roleManager, logger);
        await SeedAdminAsync(userManager, configuration, logger);
        await SeedCategoriesAsync(context, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var role in new[] { Roles.Admin, Roles.Customer })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Created role {Role}", role);
            }
        }
    }

    private static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var email = configuration["AdminSeed:Email"] ?? "admin@elyrabd.com";
        var password = configuration["AdminSeed:Password"] ?? "Admin@12345";
        var fullName = configuration["AdminSeed:FullName"] ?? "ElyraBd Administrator";

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is not null)
        {
            if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            return;
        }

        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            PhoneNumber = configuration["AdminSeed:Phone"],
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to seed admin: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);
        logger.LogInformation("Seeded admin user {Email}", email);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new[]
        {
            new Category { Name = "Electronics", Description = "Gadgets, phones, and accessories" },
            new Category { Name = "Fashion", Description = "Clothing and apparel" },
            new Category { Name = "Home & Living", Description = "Furniture and home essentials" },
            new Category { Name = "Beauty", Description = "Skincare and personal care" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} categories", categories.Length);
    }
}
