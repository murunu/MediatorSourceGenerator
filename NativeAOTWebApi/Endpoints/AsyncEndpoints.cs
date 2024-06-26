﻿using Mediator;
using TestReceivers.Async;
using TestReceivers.Void;

namespace NativeAOTWebApi.Endpoints;

public static class AsyncEndpoints
{
    public static WebApplication MapAsyncEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/async");
        
        group.MapGet("/send/success", SendSuccess);
        group.MapGet("/send/failure", SendFailure);
        group.MapGet("/publish", Publish);
        group.MapGet("/sendwithvalue/success/{page}", SendWithValueSuccess);
        group.MapGet("/sendwithvalue/failure/{page}", SendWithValueFailure);
        
        return app;
    }

    /// <summary>
    /// This message will be received by the first found service with a matching input type.
    /// The default receiver in this example writes the message to the console.
    /// </summary>
    /// <param name="mediator"></param>
    /// <returns></returns>
    private static async Task<IResult> SendSuccess(ISender mediator)
    {
        var result = await mediator.SendAsync(
            new AsyncReceiverType("A random message!"));
    
        return result.Match(
            () => Results.Ok("Only 1 message sent!"),
            Results.BadRequest
            );
    }

    /// <summary>
    /// This endpoint throws an exception because there is no receiver with int configured.
    /// </summary>
    /// <param name="mediator"></param>
    /// <returns></returns>
    private static async Task SendFailure(ISender mediator)
    {
        var result = await mediator.SendAsync(new UnregisteredReceiverType(""));
        result.ThrowIfFailure();
    }

    /// <summary>
    /// This message will be received by all receivers with a matching input type.
    /// The receivers will be executed in parallel.
    /// The default receivers in this example each write the message to the console.
    /// </summary>
    /// <param name="mediator"></param>
    /// <returns></returns>
    private static async Task<IResult> Publish(IPublisher mediator)
    {
        var result = await mediator.PublishAsync(
            new AsyncReceiverType("A random message!"));
        
        return result.Match(
            () => Results.Ok("It worked!"),
            Results.BadRequest
            );
    }
    
    /// <summary>
    /// This endpoint should return a random number appended to the message.
    /// Example url: http://localhost:5120/async/sendwithvalue/success/A random message!
    /// </summary>
    /// <param name="page"></param>
    /// <param name="mediator"></param>
    /// <returns></returns>
    private static async Task<IResult> SendWithValueSuccess(string page, ISender mediator)
    {
        var result = await mediator.SendAsync<AsyncReceiverType, AsyncReceiverResponseType>(
            new AsyncReceiverType(page));
        
        result.ThrowIfFailure();
        
        return result.Match(
            success => Results.Ok(success.ToString()),
            Results.BadRequest
            );
    }
    
    /// <summary>
    /// This endpoint throws an exception because there is no receiver with string, int configured.
    /// Example url: http://localhost:5120/async/sendwithvalue/error/A random message!
    /// </summary>
    /// <param name="page"></param>
    /// <param name="mediator"></param>
    /// <returns></returns>
    private static async Task SendWithValueFailure(string page, ISender mediator)
    {
        var result = await mediator.SendAsync<UnregisteredReceiverType, string>(new UnregisteredReceiverType(page));
        
        result.ThrowIfFailure();
    }
}