using Mediator.Exceptions;
using Mediator.Implementations.Interfaces;
using Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Implementations;

public class DefaultMediator(IServiceProvider serviceProvider)
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
        
        foreach (var service in services)
        {
            service.Receive(message);
        }
        
        return MediatorResult.Success(services.Count);
    }

    public async Task<MediatorResult> PublishAsync<T>(T message) where T : IRequest
    {
        List<Task> tasks = [];
        
        tasks.AddRange(
            serviceProvider
                .GetServices<IAsyncReceiver<T>>()
                .Select(x => x.ReceiveAsync(message)));

        tasks.Add(Task.Run(() => Publish(message)));

        await Task.WhenAll(tasks);
        
        return MediatorResult.Success(tasks.Count);
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