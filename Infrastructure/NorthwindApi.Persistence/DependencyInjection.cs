using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApi.Application.Interfaces.Repositories;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Persistence.Contexts;
using NorthwindApi.Persistence.Services;

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

        return services;
    }
}