using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Async;

[RegisterMediator]
public class AsyncReceiver : IAsyncReceiver<AsyncReceiverType>
{
    public Task<MediatorResult> ReceiveAsync(AsyncReceiverType message, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            MediatorResult.Success()
            );
    }
}

public class AsyncReceiverType(string name) : BaseHandler(name)
{
    public int Number { get; set; }
}