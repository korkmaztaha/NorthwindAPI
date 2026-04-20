using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Repositories;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Persistence.BusinessRules;
using NorthwindApi.Persistence.Contexts;
using NorthwindApi.Persistence.Repositories;
using NorthwindApi.Persistence.Services;
using NorthwindApi.Persistence.Services.EntityServices;

namespace NorthwindApi.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NorthwindDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("NorthwindConnection")));
        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICustomerBusinessRules, CustomerBusinessRules>();
        services.AddScoped<IProductBusinessRules, ProductBusinessRules>();
        services.AddScoped<IOrderBusinessRules, OrderBusinessRules>();
        services.AddScoped<IShipperBusinessRules, ShipperBusinessRules>();
        services.AddScoped<IShipperService, ShipperService>();
        services.AddScoped<ICategoryBusinessRules, CategoryBusinessRules>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISupplierBusinessRules, SupplierBusinessRules>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}