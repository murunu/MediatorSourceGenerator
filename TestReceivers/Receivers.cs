using Mediator;
using Mediator.Interfaces;

namespace TestReceivers;


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
public class AnotherRandomReceiver : IReceiver<string>
{
    public void Receive(string message)
    {
        // Handle the received message
        Console.WriteLine($"Received message: {message}");
    }
}

[RegisterMediator]
public class ResponseToo : IReceiver<string, string>
{
    public string Receive(string message)
    {
        // Handle the received message
        Console.WriteLine($"Received message: {message}");

        var random = new Random();
        return message + random.Next(int.MinValue, int.MaxValue);
    }
}