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
        private static string SendSuccess(IMediator mediator)
        {
            mediator.Send(
                new ReceiverType("A random message!"));
        
            return "Only 1 message sent!";
        }
    
        /// <summary>
        /// This endpoint throws an exception because there is no receiver with int configured.
        /// </summary>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static void SendFailure(IMediator mediator)
        {
            var result = mediator.Send(1);
            
            result.ThrowIfFailure();
        }
    
        /// <summary>
        /// This message will be received by all receivers with a matching input type.
        /// The receivers will be executed in parallel.
        /// The default receivers in this example each write the message to the console.
        /// </summary>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static string Publish(IMediator mediator)
        {
            mediator.Publish(
                new ReceiverType("A random message!"));
    
            return "It worked!";
        }
        
        /// <summary>
        /// This endpoint should return a random number appended to the message.
        /// Example url: http://localhost:5120/void/sendwithvalue/success/A random message!
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static string SendWithValueSuccess(string page, IMediator mediator)
        {
            var result = mediator.Send<ReceiverType, ReceiverResponseType>(
                new ReceiverType(page));
            
            return result.Value.ToString();
        }
        
        /// <summary>
        /// This endpoint throws an exception because there is no receiver with string, int configured.
        /// Example url: http://localhost:5120/void/sendwithvalue/error/A random message!
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        private static void SendWithValueFailure(string page, IMediator mediator)
        {
            var result = mediator.Send<string, string>(page);
            
            result.ThrowIfFailure();
        }
}