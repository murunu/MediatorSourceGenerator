using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Void;

[RegisterMediator]
public class Receiver : IReceiver<ReceiverType>
{
    public MediatorResult Receive(ReceiverType message)
    {
        // Handle the received message
        Console.WriteLine($"Received at ConcreteReceiver message: {message}");

        return MediatorResult.Success();
    }
}

public class ReceiverType(string name) : BaseHandler(name);

public class UnregisteredReceiverType(string name) : BaseHandler(name);