using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContadorTabaco.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cgf => cgf.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
