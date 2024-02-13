using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Async;

[RegisterMediator]
public class AsyncReceiverWithException : IAsyncReceiver<AsyncReceiverWithExceptionType>
{
    public async Task<MediatorResult> ReceiveAsync(AsyncReceiverWithExceptionType message, CancellationToken cancellationToken = default)
    {
        return new Exception("Oh no! An exception occurred!");
    }
}

[RegisterMediator]
public class AsyncReceiverWithException2 : IAsyncReceiver<AsyncReceiverWithExceptionType>
{
    public Task<MediatorResult> ReceiveAsync(AsyncReceiverWithExceptionType message, CancellationToken cancellationToken = default)
    {
        throw new Exception("Oh no! An exception occurred!");
    }
}

public class AsyncReceiverWithExceptionType(string name) : BaseHandler(name)
{
    public string Name { get; set; }
}