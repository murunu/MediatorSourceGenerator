using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Async;

[RegisterMediator]
public class AsyncReceiver : IAsyncReceiver<AsyncReceiverType>
{
    public async Task ReceiveAsync(AsyncReceiverType message)
    {
        // Handle the received message
        Console.WriteLine($"Received async message at ConcreteAsyncReceiver: {message}");
    }
}

public class AsyncReceiverType(string name) : BaseHandler(name)
{
    public int Number { get; set; }
}