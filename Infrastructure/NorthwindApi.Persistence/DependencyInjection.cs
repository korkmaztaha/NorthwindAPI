using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Repositories;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Persistence.BusinessRules;
using NorthwindApi.Persistence.Contexts;
using NorthwindApi.Persistence.Jobs;
using NorthwindApi.Persistence.Repositories;
using NorthwindApi.Persistence.Services;
using NorthwindApi.Persistence.Services.EntityServices;
using StackExchange.Redis;

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
        services.AddScoped<IEmployeeBusinessRules, EmployeeBusinessRules>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = configuration["Redis:ConnectionString"];
            return ConnectionMultiplexer.Connect(config!);
        });
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOrderNotificationJob, OrderNotificationJob>();
        services.AddScoped<IBasketService, BasketService>();

        return services;
    }
}