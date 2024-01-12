namespace Mediator;

public interface IMediator
{
    void Send<T>(T message);
    Task SendAsync<T>(T message);
    void Publish<T>(T message);
    Task PublishAsync<T>(T message);
    
    TOutput Send<T, TOutput>(T message);
    Task<TOutput> SendAsync<T, TOutput>(T message);
}