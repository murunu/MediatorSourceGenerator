using Mediator.Interfaces;

namespace Mediator.Middleware.Interface;

public interface IMediatorMiddleware
{
    void Run<TRequest>(TRequest data) where TRequest : IRequest;
}