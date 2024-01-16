using Mediator.Implementations;
using Mediator.Implementations.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class MediatorConfiguration(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
}

public static class Services
{
    public static MediatorConfiguration AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, BaseMediator>();
        
        return new MediatorConfiguration(services);
    }
    
    public static MediatorConfiguration AddDefaultMediator(this MediatorConfiguration services)
    {
        services.Services.AddScoped<IMediatorImplementation, DefaultMediator>();
        return services;
    }
}