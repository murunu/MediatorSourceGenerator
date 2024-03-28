# Mediator
Simple [Mediator pattern](https://en.wikipedia.org/wiki/Mediator_pattern) implementation in .NET
Heavily inspired by [MediatR](https://github.com/jbogard/MediatR).

NativeAOT compatible, as it uses source generators instead of reflection.

Supports request/response and publishing of messages.

## Disclaimer
Be aware this package is not meant for production use, as it is just made as a side-project.  
Any bugs can be reported as a bug report in the [issues](https://github.com/murunu/MediatorSourceGenerator/issues) panel.

## Installing
```
Install-Package Murunu.Mediator
```

## Usage
Registering default Mediator implementation:

To register the default Mediator implementation implement the following in your `Program.cs` file.

```csharp
builder.Services.AddMediator();
```

This will implement all default services needed for the Mediator.
To Register a receiver, add a scoped service to the DI container like following.

```csharp
builder.Services.AddScoped<I(Async)Receiver, {YourReceiverType}>();
```

Make sure your implementation implements either `IReceiver` or `IAsyncReceiver`.
See [Implementation](#implementation) for more information.

## Implementation
There is multiple choices for implementing a receiver. Make sure you choose the right one for your use case.

The main choices are between `Sync` and `Async` receivers.

## Source Generator
If using the source generator, you do not have to add the receivers to the DI container.  
The only thing that needs to be added for the source generator receivers is the `[RegisterMediator]` attribute.

Make sure to add the following to `Program.cs`

```csharp
builder.Services.AddMediator()
    .AddSourceGenerator();
```

This adds all needed services to make the Mediator work with the source generator.