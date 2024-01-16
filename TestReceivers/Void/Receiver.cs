using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Void;

[RegisterMediator]
public class Receiver : IReceiver<ReceiverType>
{
    public void Receive(ReceiverType message)
    {
        // Handle the received message
        Console.WriteLine($"Received at ConcreteReceiver message: {message}");
    }
}

public class ReceiverType(string name) : BaseHandler(name);