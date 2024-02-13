using Mediator.Exceptions;
using Mediator.Implementations.Interfaces;
using Mediator.Interfaces;
using Mediator.Middleware.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class BasePublisher(IServiceProvider serviceProvider, IMediatorMiddleware middleware) : IPublisher
{
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
}