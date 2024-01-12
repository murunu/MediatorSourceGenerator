namespace Mediator.Exceptions;

public class NoServiceException : Exception
{
    public NoServiceException(Type type) :
        base($"No service for type IReceiver<{type.FullName}> or IReceiverAsync<{type.FullName}> has been registered.")
    {
        
    }
    
    public NoServiceException(Type inputType, Type outputType) :
        base($"No service for type IReceiver<{inputType.FullName}, {outputType.FullName}> or IReceiverAsync<{inputType.FullName}, {outputType.FullName}> has been registered.")
    {
        
    }
}