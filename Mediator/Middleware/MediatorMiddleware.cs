using Mediator.Interfaces;
using Mediator.Middleware.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class MediatorMiddleware(IServiceProvider serviceProvider) : IMediatorMiddleware
{
    public void Run<TRequest>(TRequest data) where TRequest : IRequest
    {
        var requestMiddlewares = serviceProvider
            .GetServices<IRequestMiddleware>()
            .ToList();

        if (requestMiddlewares is not { Count: > 0 })
        {
            return;
        }

        Next(data, 0);
            
        return;

        void Next(TRequest request, int index)
        {
            if (index == requestMiddlewares.Count)
            {
                return;
            }

            var currentMiddleware = requestMiddlewares[index];
            currentMiddleware.Handle(
                request, 
                next => Next(next, index + 1));
        }
    }
}