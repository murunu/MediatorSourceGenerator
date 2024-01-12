using Mediator.Exceptions;
using Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mediator;

public class MediatorBase(IServiceProvider serviceProvider, IOptions<MediatorOptions> options)
    : IMediator
{
    private readonly MediatorOptions _options = options.Value;

    public void Send<T>(T message)
    {
        var receiverType = _options.Receivers
            .Where(x => x.InputType == typeof(T) && x is
            {
                IsAsync: false,
                HasResponse: false
            })
            .Select(x => x.Type).FirstOrDefault();
        
        if (receiverType == null || serviceProvider.GetRequiredService(receiverType) is not IReceiver<T> receiver)
        {
            throw new NoServiceException(typeof(T));
        }
            
        receiver.Receive(message);
    }

    public Task SendAsync<T>(T message)
    {
        var receiverType = _options.Receivers
            .Where(x => x.InputType == typeof(T) && x is
            {
                IsAsync: true,
                HasResponse: false
            })
            .Select(x => x.Type).FirstOrDefault();
        
        if (receiverType == null || serviceProvider.GetRequiredService(receiverType) is not IAsyncReceiver<T> receiver)
        {
            Send(message);
            return Task.CompletedTask;
        }
            
        return receiver.ReceiveAsync(message);
    }
    
    public TOutput Send<T, TOutput>(T message)
    {
        var receiverType = _options.Receivers
            .Where(x => x.InputType.FullName == typeof(T).FullName && x is
            {
                IsAsync: false,
                HasResponse: true
            })
            .Select(x => x.Type)
            .FirstOrDefault();
        
        if (receiverType == null || 
            serviceProvider.GetRequiredService(receiverType) is not IReceiver<T, TOutput> receiver)
        {
            throw new NoServiceException(typeof(T), typeof(TOutput));
        }
        
        return receiver.Receive(message);
    }
    
    public Task<TOutput> SendAsync<T, TOutput>(T message)
    {
        var receiverType = _options.Receivers
            .Where(x => x.InputType.FullName == typeof(T).FullName && x is
            {
                IsAsync: true,
                HasResponse: true
            })
            .Select(x => x.Type)
            .FirstOrDefault();
        
        if (receiverType == null || serviceProvider.GetRequiredService(receiverType) is not IAsyncReceiver<T, TOutput> receiver)
        {
            return Task.FromResult(Send<T, TOutput>(message));
        }
            
        return receiver.ReceiveAsync(message);
    }
    
    public void Publish<T>(T message)
    {
        foreach (var receiverType in _options.Receivers
                     .Where(x => x.InputType == typeof(T) && x is
                     {
                         IsAsync: false,
                         HasResponse: false
                     })
                     .Select(x => x.Type))
        {
            if (serviceProvider.GetRequiredService(receiverType) is not IReceiver<T> receiver)
            {
                continue;
            }
            
            receiver.Receive(message);
        }
    }
    
    public Task PublishAsync<T>(T message)
    {
        List<Task> tasks = [];
        
        foreach (var receiverType in _options.Receivers
                     .Where(x => x.InputType == typeof(T) && x is
                     {
                         IsAsync: true,
                         HasResponse: false
                     })
                     .Select(x => x.Type))
        {
            if (serviceProvider.GetRequiredService(receiverType) is not IAsyncReceiver<T> receiver)
            {
                continue;
            }
            
            tasks.Add(receiver.ReceiveAsync(message));
        }

        tasks.Add(
            Task.Run(() => Publish(message)));
        
        return Task.WhenAll(tasks);
    }
}