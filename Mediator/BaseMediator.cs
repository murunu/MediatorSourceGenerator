using Mediator.Exceptions;
using Mediator.Implementations.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class BaseMediator(IServiceProvider serviceProvider)
    : IMediator
{
    public MediatorResult Send<T>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        foreach (var service in services)
        {
            var result = service.Send(message);

            return result;
        }

        return MediatorResult.Failure(new NoImplementationException());
    }

    public async Task<MediatorResult> SendAsync<T>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        foreach (var service in services)
        {
            var result = await service.SendAsync(message);

            return result;
        }

        return MediatorResult.Failure(new NoImplementationException());
    }

    public MediatorResult Publish<T>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }

        var results = services.Select(x => x.Publish(message)).ToList();

        var result = results.FirstOrDefault();
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

    public async Task<MediatorResult> PublishAsync<T>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }

        var tasks = services.Select(x => x.PublishAsync(message)).ToList();

        await Task.WhenAll(tasks);
        var results = tasks.Select(x => x.Result).ToList();

        var result = results.FirstOrDefault();
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

    public MediatorResult<TOutput> Send<T, TOutput>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        foreach (var service in services)
        {
            var result = service.Send<T, TOutput>(message);

            return result;
        }

        return new NoImplementationException();
    }

    public async Task<MediatorResult<TOutput>> SendAsync<T, TOutput>(T message)
    {
        var services = serviceProvider.GetServices<IMediatorImplementation>().ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }
        
        foreach (var service in services)
        {
            var result = await service.SendAsync<T, TOutput>(message);
            
            return result;
        }

        return new NoImplementationException();
    }
}