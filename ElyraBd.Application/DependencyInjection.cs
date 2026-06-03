using ElyraBd.Application.Interfaces;
using ElyraBd.Application.Mappings;
using ElyraBd.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ElyraBd.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IActivityTrackingService, ActivityTrackingService>();
        return services;
    }
}
