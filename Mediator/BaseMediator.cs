using Mediator.Exceptions;
using Mediator.Implementations.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class BaseMediator(IServiceProvider serviceProvider)
    : IMediator
{
    public void Send<T>(T message)
    {
        var service = serviceProvider.GetService<IMediatorImplementation>();
        if (service == null)
        {
            throw new NoImplementationException();
        }
        
        service.Send(message);
    }

    public Task SendAsync<T>(T message)
    {
        var service = serviceProvider.GetService<IMediatorImplementation>();
        if (service == null)
        {
            throw new NoImplementationException();
        }
        
        return service.SendAsync(message);
    }

    public void Publish<T>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        foreach (var service in services)
        {
            service.Publish(message);
        }
    }

    public async Task PublishAsync<T>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        foreach (var service in services)
        {
            await service.PublishAsync(message);
        }
    }

    public TOutput Send<T, TOutput>(T message)
    {
        var service = serviceProvider.GetService<IMediatorImplementation>();
        if (service == null)
        {
            throw new NoImplementationException();
        }
        
        return service.Send<T, TOutput>(message);
    }

    public Task<TOutput> SendAsync<T, TOutput>(T message)
    {
        var service = serviceProvider.GetService<IMediatorImplementation>();
        if (service == null)
        {
            throw new NoImplementationException();
        }
        
        return service.SendAsync<T, TOutput>(message);
    }
}