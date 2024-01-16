﻿using Mediator;
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

public class AsyncReceiverType : BaseHandler
{
    public AsyncReceiverType(string name) : base(name)
    {
        
    }
}