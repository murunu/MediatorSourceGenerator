using Mediator.Exceptions;
using Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class MediatorBase(IServiceProvider serviceProvider)
    : IMediator
{
    public void Send<T>(T message)
    {
        var service = serviceProvider.GetService<IReceiver<T>>();
        if (service == null)
        {
            throw new NoServiceException(typeof(T));
        }
        
        service.Receive(message);
    }

    public async Task SendAsync<T>(T message)
    {
        var service = serviceProvider.GetService<IAsyncReceiver<T>>();
        if (service == null)
        {
            Send(message);

            return;
        }
        
        await service.ReceiveAsync(message);
    }

    public void Publish<T>(T message)
    {
        var services = serviceProvider.GetServices<IReceiver<T>>();
        
        foreach (var service in services)
        {
            service.Receive(message);
        }
    }

    public Task PublishAsync<T>(T message)
    {
        List<Task> tasks = [];
        
        tasks.AddRange(serviceProvider.GetServices<IAsyncReceiver<T>>().Select(x => x.ReceiveAsync(message)));

        tasks.Add(Task.Run(() => Publish(message)));
        
        return Task.WhenAll(tasks);
    }

    public TOutput Send<T, TOutput>(T message)
    {
        var service = serviceProvider.GetService<IReceiver<T, TOutput>>();
        
        if (service == null)
        {
            throw new NoServiceException(typeof(T));
        }
        
        return service.Receive(message);
    }

    public Task<TOutput> SendAsync<T, TOutput>(T message)
    {
        var service = serviceProvider.GetService<IAsyncReceiver<T, TOutput>>();

        if (service == null)
        {
            return Task.FromResult(Send<T, TOutput>(message));
        }
        
        return service.ReceiveAsync(message);
    }
}