using Mediator;
using TestReceivers.Void;

namespace NativeAOTWebApi.Endpoints;

public static class VoidEndpoints
{
     public static WebApplication MapVoidEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/void");
            
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
        private static IResult SendSuccess(ISender mediator)
        {
            var result = mediator.Send(
                new ReceiverType("A random message!"));
        
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
        private static void SendFailure(ISender mediator)
        {
            var result = mediator.Send(new UnregisteredReceiverType(""));
            
            result.ThrowIfFailure();
        }
    
        /// <summary>
        /// This message will be received by all receivers with a matching input type.
        /// The receivers will be executed in parallel.
        /// The default receivers in this example each write the message to the console.
        /// </summary>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static IResult Publish(IPublisher mediator)
        {
            var result = mediator.Publish(
                new ReceiverType("A random message!"));
    
            return result.Match(
                () => Results.Ok("It worked!"),
                Results.BadRequest
                );
        }
        
        /// <summary>
        /// This endpoint should return a random number appended to the message.
        /// Example url: http://localhost:5120/void/sendwithvalue/success/A random message!
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static IResult SendWithValueSuccess(string page, ISender mediator)
        {
            var result = mediator.Send<ReceiverType, ReceiverResponseType>(
                new ReceiverType(page));
            
            result.ThrowIfFailure();

            return result.Match(
                success => Results.Ok(success.ToString()),
                Results.BadRequest
            );
        }
        
        /// <summary>
        /// This endpoint throws an exception because there is no receiver with string, int configured.
        /// Example url: http://localhost:5120/void/sendwithvalue/error/A random message!
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static void SendWithValueFailure(string page, ISender mediator)
        {
            var result = mediator.Send<UnregisteredReceiverType, string>(
                new UnregisteredReceiverType(page));
            
            result.ThrowIfFailure();
        }
}