using Mediator.Implementations;
using Mediator.Implementations.Interfaces;
using Mediator.Middleware.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        services.AddScoped<ISender, BaseSender>();
        services.AddScoped<IPublisher, BasePublisher>();
        
        services.AddScoped<IMediatorMiddleware, MediatorMiddleware>();
        
        return new MediatorConfiguration(services);
    }
    
    public static MediatorConfiguration AddDefaultMediator(this MediatorConfiguration services)
    {
        services.Services.AddScoped<IMediatorImplementation, DefaultMediatorImplementation>();
        
        return services;
    }

    public static MediatorConfiguration AddMediatorMiddleware<TMiddleware>(this MediatorConfiguration services)
        where TMiddleware : class, IRequestMiddleware =>
        AddMediatorMiddleWare(services, typeof(TMiddleware));
    
    public static MediatorConfiguration AddMediatorMiddleWare(this MediatorConfiguration services, Type middlewareType)
    {
        services.Services.AddScoped(typeof(IRequestMiddleware), middlewareType);
        return services;
    }
}