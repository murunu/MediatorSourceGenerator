using Mediator.Exceptions;
using Mediator.Implementations.Interfaces;
using Mediator.Interfaces;
using Mediator.Middleware.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class BaseMediator(IServiceProvider serviceProvider, IMediatorMiddleware middleware)
    : IMediator
{
    public MediatorResult Send<T>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }

        middleware.Run(message);

        foreach (var service in services)
        {
            var result = service.Send(message);

            return result;
        }

        return MediatorResult.Failure(new NoImplementationException());
    }

    public async Task<MediatorResult> SendAsync<T>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        middleware.Run(message);

        foreach (var service in services)
        {
            var result = await service.SendAsync(message);

            return result;
        }

        return MediatorResult.Failure(new NoImplementationException());
    }

    public MediatorResult Publish<T>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }

        middleware.Run(message);

        var results = services.Select(x => x.Publish(message)).ToList();

        MediatorResult? result = null;
        foreach (var item in results)
        {
            if (result == null)
            {
                result = item;
                continue;
            }
            
            result += item;
        }
        
        return result ?? MediatorResult.Failure(new NoImplementationException());
    }

    public async Task<MediatorResult> PublishAsync<T>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        middleware.Run(message);

        var tasks = services.Select(x => x.PublishAsync(message)).ToList();

        await Task.WhenAll(tasks);
        var results = tasks.Select(x => x.Result).ToList();

        MediatorResult? result = null;
        foreach (var item in results)
        {
            if (result == null)
            {
                result = item;
                continue;
            }
            
            result += item;
        }
        
        return result ?? MediatorResult.Failure(new NoImplementationException());
    }

    public MediatorResult<TOutput> Send<T, TOutput>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        middleware.Run(message);
        
        foreach (var service in services)
        {
            var result = service.Send<T, TOutput>(message);

            return result;
        }

        return new NoImplementationException();
    }

    public async Task<MediatorResult<TOutput>> SendAsync<T, TOutput>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        middleware.Run(message);
        
        foreach (var service in services)
        {
            var result = await service.SendAsync<T, TOutput>(message);
            
            return result;
        }

        return new NoImplementationException();
    }
}