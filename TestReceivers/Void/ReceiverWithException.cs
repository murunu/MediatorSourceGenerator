using Mediator;
using Mediator.Interfaces;

namespace TestReceivers.Void;

[RegisterMediator]
public class ReceiverWithException : IReceiver<ReceiverWithExceptionType>
{
    public MediatorResult Receive(ReceiverWithExceptionType message)
    {
        return new Exception("Oh no! An exception occurred!");
    }
}

[RegisterMediator]
public class ReceiverWithException2 : IReceiver<ReceiverWithExceptionType>
{
    public MediatorResult Receive(ReceiverWithExceptionType message)
    {
        throw new Exception("Oh no! An exception occurred!");
    }
}


public class ReceiverWithExceptionType(string name) : BaseHandler(name)
{
    public string Name { get; set; }
}