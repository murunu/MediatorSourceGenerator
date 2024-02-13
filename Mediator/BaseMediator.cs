using Mediator.Interfaces;

namespace Mediator;

public class BaseMediator(
    ISender sender, 
    IPublisher publisher) : IMediator
{
    public MediatorResult Publish<T>(T message) where T : IRequest
    {
        return publisher.Publish(message);
    }

    public Task<MediatorResult> PublishAsync<T>(T message) where T : IRequest
    {
        return publisher.PublishAsync(message);
    }

    public MediatorResult Send<T>(T message) where T : IRequest
    {
        return sender.Send(message);
    }

    public Task<MediatorResult> SendAsync<T>(T message) where T : IRequest
    {
        return sender.SendAsync(message);
    }

    public MediatorResult<TOutput> Send<T, TOutput>(T message) where T : IRequest
    {
        return sender.Send<T, TOutput>(message);
    }

    public Task<MediatorResult<TOutput>> SendAsync<T, TOutput>(T message) where T : IRequest
    {
        return sender.SendAsync<T, TOutput>(message);
    }
}
