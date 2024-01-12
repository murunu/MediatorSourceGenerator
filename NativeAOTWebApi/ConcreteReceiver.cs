using Mediator.Interfaces;
using Mediator;

namespace NativeAOTWebApi;

[RegisterMediator]
public class ConcreteAsyncReceiver : IAsyncReceiver<string>
{
    public async Task ReceiveAsync(string message)
    {
        // Handle the received message
        Console.WriteLine($"Received async message: {message}");
    }
}


[RegisterMediator]
public class ConcreteReceiver : IReceiver<string>
{
    public void Receive(string message)
    {
        // Handle the received message
        Console.WriteLine($"Received message: {message}");
    }
}


[RegisterMediator]
public class jkdlsfjdklgkljfkljdjkldgfkljgdf : IReceiver<string>
{
    public void Receive(string message)
    {
        // Handle the received message
        Console.WriteLine($"Received message: {message}");
    }
}