namespace Mediator.Interfaces;

public interface IReceiver<in T>
{
    MediatorResult Receive(T message);
}

public interface IAsyncReceiver<in T>
{
    Task<MediatorResult> ReceiveAsync(T message, CancellationToken cancellationToken = default);
}

public interface IReceiver<in T, out TOutput>
{
    TOutput Receive(T message);
}

public interface IAsyncReceiver<in T, TOutput>
{
    Task<TOutput> ReceiveAsync(T message);
}