using Mediator;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediator();

var app = builder.Build();

// This message will be received by the first found service with a matching input type.
// The default receiver in this example writes the message to the console.
app.MapGet("/send/success", async (IMediator mediator) =>
{
    await mediator.SendAsync("A random message!");
    return "Only 1 message sent!";
});

// This endpoint throws an exception because there is no receiver with <int> configured.
app.MapGet("/send/error", async (IMediator mediator) =>
{
    await mediator.SendAsync(1);
    return "Oops, something went wrong";
});

// This message will be received by all receivers with a matching input type.
// The receivers will be executed in parallel.
// The default receivers in this example each write the message to the console.
app.MapGet("/publish", async (IMediator mediator) =>
{
    await mediator.PublishAsync("A random message!");

    return "It worked!";
});

// This endpoint should return a random number appended to the message.
// Example url: http://localhost:5120/sendwithvalue/success?page=A random message!
app.MapGet("/sendwithvalue/success", async (string page, IMediator mediator) 
    => await mediator.SendAsync<string, string>(page));

// This endpoint throws an exception because there is no receiver with <string, int> configured.
// Example url: http://localhost:5120/sendwithvalue/error?page=A random message!
app.MapGet("/sendwithvalue/error", async (string page, IMediator mediator) 
    => (await mediator.SendAsync<string, int>(page)).ToString());

app.Run();