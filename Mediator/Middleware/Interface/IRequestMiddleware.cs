using Mediator.Interfaces;
using Mediator.Middleware;

namespace Mediator;

public interface IRequestMiddleware
{
    void Handle<TRequest>(TRequest request, RequestHandlerDelegate<TRequest> next) 
        where TRequest : IRequest;
}