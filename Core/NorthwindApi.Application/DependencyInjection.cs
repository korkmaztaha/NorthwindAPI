using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApi.Application.Behaviors;
using System.Reflection;

using NorthwindApi.Application.Mapping;


namespace NorthwindApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        
        services.AddAutoMapper(cfg => 
        {
            cfg.AddProfile<CustomerProfile>();
        }, Assembly.GetExecutingAssembly());

        return services;
    }
}