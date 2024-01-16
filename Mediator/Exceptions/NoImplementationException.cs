namespace Mediator.Exceptions;

public class NoImplementationException : Exception
{
    public NoImplementationException() :
        base("No implementation for IMediator has been registered.")
    {
        
    }
}