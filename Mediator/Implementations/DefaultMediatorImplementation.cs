using Mediator.Exceptions;
using Mediator.Implementations.Interfaces;
using Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Implementations;

public class DefaultMediatorImplementation(IServiceProvider serviceProvider)
    : IMediatorImplementation
{
    public MediatorResult Send<T>(T message) where T : IRequest
    {
        var service = serviceProvider.GetService<IReceiver<T>>();
        if (service == null)
        {
            return new NoServiceException(typeof(T));
        }
        
        service.Receive(message);

        return MediatorResult.Success();
    }

    public async Task<MediatorResult> SendAsync<T>(T message) where T : IRequest
    {
        var service = serviceProvider.GetService<IAsyncReceiver<T>>();
        if (service == null)
        {
            return Send(message);
        }
        
        await service.ReceiveAsync(message);

        return MediatorResult.Success();
    }

    public MediatorResult Publish<T>(T message) where T : IRequest
    {
        var services = serviceProvider.GetServices<IReceiver<T>>().ToList();

        if (services is not { Count: > 0 })
        {
            throw new NoImplementationException();
        }
        
        List<MediatorResult> results = [];
        foreach (var service in services)
        {
            try
            { 
                var result = service.Receive(message);

                results.Add(result);
            }
            catch (Exception e)
            {
                results.Add(e);
            }
        }
        
        return results.Aggregate((currentResult, nextResult) => currentResult + nextResult);
    }

    public async Task<MediatorResult> PublishAsync<T>(T message, CancellationToken cancellationToken) where T : IRequest
    {
        List<Task> tasks = [];
        var services = serviceProvider.GetServices<IAsyncReceiver<T>>().ToList();

        if(services is not {Count: > 0})
        {
            throw new NoImplementationException();
        }
        
        List<MediatorResult> results = [];
        
        await Parallel.ForEachAsync(services, cancellationToken, async (service, _) =>
        {
            try
            {
                var result = await service.ReceiveAsync(message, cancellationToken);
                
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
            results.Add(syncResult);
        }
        catch (NoImplementationException e)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            results.Add(e);
        }

        return results.Aggregate((currentResult, nextResult) => currentResult + nextResult);
    }

    public MediatorResult<TOutput> Send<T, TOutput>(T message) where T : IRequest
    {
        var service = serviceProvider.GetService<IReceiver<T, TOutput>>();
        
        if (service == null)
        {
            return new NoServiceException(typeof(T), typeof(TOutput));
        }
        
        return service.Receive(message);
    }

    public async Task<MediatorResult<TOutput>> SendAsync<T, TOutput>(T message) where T : IRequest
    {
        var service = serviceProvider.GetService<IAsyncReceiver<T, TOutput>>();

        if (service == null)
        {
            return await Task.FromResult(Send<T, TOutput>(message));
        }
        
        return await service.ReceiveAsync(message);
    }
}