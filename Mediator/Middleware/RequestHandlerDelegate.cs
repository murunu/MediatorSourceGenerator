using Mediator.Interfaces;

namespace Mediator.Middleware;

public delegate void RequestHandlerDelegate<in TRequest>(TRequest request) where TRequest : IRequest;
