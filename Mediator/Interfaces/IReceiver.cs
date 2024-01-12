namespace Mediator.Interfaces;

public interface IReceiver<in T>
{
    void Receive(T message);
}

public interface IAsyncReceiver<in T>
{
    Task ReceiveAsync(T message);
}

public interface IReceiver<in T, out TOutput>
{
    TOutput Receive(T message);
}

public interface IAsyncReceiver<in T, TOutput>
{
    Task<TOutput> ReceiveAsync(T message);
}