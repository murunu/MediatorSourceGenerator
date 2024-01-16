using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Async;

[RegisterMediator]
public class AsyncReceiverWithReturn : IAsyncReceiver<AsyncReceiverType, AsyncReceiverResponseType>
{
    public async Task<AsyncReceiverResponseType> ReceiveAsync(AsyncReceiverType message)
    {
        Console.WriteLine($"Received message: {message}");

        return new AsyncReceiverResponseType(message.Name);
    }
}

public class AsyncReceiverResponseType : BaseHandler
{
    public AsyncReceiverResponseType(string name) : base(name)
    {
        
    }
}
