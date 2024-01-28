using Mediator.Interfaces;
using Mediator.Middleware;
using Mediator.UnitTests.Fixtures;
using TestReceivers.Async;
using TestReceivers.Void;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Mediator.UnitTests;

public class MiddleWareTest : TestBed<MiddlewareTestFixture>
{
    private readonly IMediator _mediator;

    public MiddleWareTest(ITestOutputHelper testOutputHelper, MiddlewareTestFixture fixture) : base(testOutputHelper, fixture)
    {
        _mediator = _fixture.GetService<IMediator>(_testOutputHelper);
    }
    
    [Fact]
    public async Task SendAsyncShouldRunSuccessfully()
    {
        var result = await _mediator.SendAsync<AsyncReceiverType, AsyncReceiverResponseType>(new AsyncReceiverType(string.Empty));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Number);
    }
    
    [Fact]
    public async Task SendAsyncWithOtherClassShouldRunSuccessfully()
    {
        var result = await _mediator.SendAsync<ReceiverType, ReceiverResponseType>(new ReceiverType(string.Empty));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(0, result.Value.Number);
    }
}

public class RequestMiddleWare1 : IRequestMiddleware
{
    public void Handle<TRequest>(TRequest request, RequestHandlerDelegate<TRequest> next) where TRequest : IRequest
    {
        if(request is not AsyncReceiverType data)
        {
            next(request);
            
            return;
        };
        
        data.Number++;

        next(request);
    }
}

public class RequestMiddleWare2 : IRequestMiddleware
{
    public void Handle<TRequest>(TRequest request, RequestHandlerDelegate<TRequest> next) where TRequest : IRequest
    {
        if(request is not AsyncReceiverType data)
        {
            next(request);
            
            return;
        };
        
        data.Number++;

        next(request);
    }
}