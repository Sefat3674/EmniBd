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
        await SeedCouponsAsync(context, logger);
        //await SeedSampleProductsAsync(context, logger);
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
            new Category { Name = "Cosmetics", Description = "Makeup, lipstick, and beauty essentials" },
            new Category { Name = "Women's Fashion", Description = "Dresses, bags, and accessories" },
            new Category { Name = "Household", Description = "Home and kitchen essentials" },
            new Category { Name = "Skincare", Description = "Serums, creams, and face care" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} categories", categories.Length);
    }

    private static async Task SeedCouponsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Coupons.AnyAsync())
            return;

        context.Coupons.AddRange(
            new Coupon
            {
                Code = "WELCOME10",
                Description = "10% off your first order",
                DiscountPercent = 10,
                MinOrderAmount = 500,
                MaxUses = 1000,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            },
            new Coupon
            {
                Code = "FLAT100",
                Description = "৳100 off orders over ৳1500",
                DiscountAmount = 100,
                MinOrderAmount = 1500,
                MaxUses = 500,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            });

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded sample coupons");
    }

    //private static async Task SeedSampleProductsAsync(ApplicationDbContext context, ILogger logger)
    //{
    //    if (await context.Products.AnyAsync())
    //        return;

    //    var categories = await context.Categories.ToListAsync();
    //    var cosmetics = categories.First(c => c.Name == "Cosmetics");
    //    var fashion = categories.First(c => c.Name == "Women's Fashion");
    //    var household = categories.First(c => c.Name == "Household");
    //    var skincare = categories.First(c => c.Name == "Skincare");

    //    var products = new List<Product>
    //    {
    //        new() { Name = "Matte Lipstick Set", Description = "Long-lasting matte lipstick trio — perfect daily wear.", Price = 1200, DiscountPercent = 15, Stock = 50, CategoryId = cosmetics.Id, IsFeatured = true, IsActive = true },
    //        new() { Name = "Floral Summer Dress", Description = "Light cotton midi dress for summer occasions.", Price = 2890, DiscountPercent = 20, Stock = 30, CategoryId = fashion.Id, IsFeatured = true, IsActive = true },
    //        new() { Name = "Kitchen Storage Set", Description = "BPA-free containers for organized pantry storage.", Price = 1599, DiscountPercent = 0, Stock = 40, CategoryId = household.Id, IsActive = true },
    //        new() { Name = "Vitamin C Face Serum", Description = "Brightening serum with hyaluronic acid.", Price = 1850, DiscountPercent = 10, Stock = 60, CategoryId = skincare.Id, IsFeatured = true, IsActive = true },
    //        new() { Name = "Designer Handbag", Description = "Premium faux-leather handbag with gold hardware.", Price = 3500, DiscountPercent = 25, Stock = 15, CategoryId = fashion.Id, IsActive = true },
    //        new() { Name = "LED Desk Lamp", Description = "Adjustable warm/cool light for home office.", Price = 2200, DiscountPercent = 5, Stock = 25, CategoryId = household.Id, IsActive = true }
    //    };

    //    context.Products.AddRange(products);
    //    await context.SaveChangesAsync();
    //    logger.LogInformation("Seeded {Count} sample products", products.Count);
    //}
}
