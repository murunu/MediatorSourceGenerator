using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Void;

[RegisterMediator]
public class ReceiverWithReturn : IReceiver<ReceiverType, ReceiverResponseType>
{
    public ReceiverResponseType Receive(ReceiverType message)
    {
        // Handle the received message
        Console.WriteLine($"Received message: {message}");

        return new ReceiverResponseType(message.Name);
    }
}

public class ReceiverResponseType(string name) : BaseHandler(name)
{
    public int Number { get; }
}