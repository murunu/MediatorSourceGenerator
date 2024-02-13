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

        try
        {
            middleware.Run(message);
        }
        catch (Exception e)
        {
            return e;
        }

        List<MediatorResult> results = [];
        foreach (var service in services)
        {
            try
            { 
                var result = service.Publish(message);

                results.Add(result);
            }
            catch (Exception e)
            {
                results.Add(e);
            }
        }
        
        return results.Aggregate(AggregateResult);
    }

    public async Task<MediatorResult> PublishAsync<T>(T message, CancellationToken cancellationToken) where T : IRequest
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var services = serviceProvider.GetServices<IMediatorImplementation>()
            .ToList();
        if (services is {Count: 0})
        {
            throw new NoImplementationException();
        }

        try
        {
            middleware.Run(message);
        }
        catch (Exception e)
        {
            return e;
        }

        List<MediatorResult> results = [];
        
        await Parallel.ForEachAsync(services, cancellationToken, async (service, _) =>
        {
            try
            {
                var result = await service.PublishAsync(message, cancellationToken);
                
                results.Add(result);
            }
            catch (Exception e)
            {
                results.Add(e);
            }
        });

        try
        {
            var syncResult = Publish(message);
            if (syncResult is not { IsFailure: true, Exceptions: [NoImplementationException] })
            {
                results.Add(syncResult);
            }
        }
        catch (NoImplementationException e)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            results.Add(e);
        }
        
        return results.Aggregate(AggregateResult);
    }
    
    private static MediatorResult AggregateResult(MediatorResult currentResult, MediatorResult nextResult) => 
            currentResult + nextResult;
}