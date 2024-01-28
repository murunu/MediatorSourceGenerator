using Mediator.Exceptions;
using Mediator.UnitTests.Fixtures;
using TestReceivers.Async;
using TestReceivers.Void;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Mediator.UnitTests;

public class NoImplementationMediatorTest : TestBed<NoImplementationMediatorTestFixture>
{
    private readonly IMediator _mediator;
    
    public NoImplementationMediatorTest(
        ITestOutputHelper testOutputHelper, 
        NoImplementationMediatorTestFixture fixture)
        : base(testOutputHelper, fixture)
    {
        _mediator = _fixture.GetService<IMediator>(_testOutputHelper);
    }
    
    [Fact]
    public async Task SendAsyncShouldThrowNoImplementationException()
    {
        await Assert.ThrowsAsync<NoImplementationException>(
            () => _mediator.SendAsync(new AsyncReceiverType("Hello World!")));
    }
    
    [Fact]
    public void SendShouldThrowNoImplementationException()
    {
        Assert.Throws<NoImplementationException>(
            () => _mediator.Send(new ReceiverType("Hello World!")));
    }
    
    [Fact]
    public async Task PublishAsyncShouldThrowNoImplementationException()
    {
        await Assert.ThrowsAsync<NoImplementationException>(
            () => _mediator.PublishAsync(new AsyncReceiverType("Hello World!")));
    }
    
    [Fact]
    public void PublishShouldThrowNoImplementationException()
    {
        Assert.Throws<NoImplementationException>(
            () => _mediator.Publish(new ReceiverType("Hello World!")));
    }
    
    [Fact]
    public async Task SendAsyncWithResponseShouldThrowNoImplementationException()
    {
        await Assert.ThrowsAsync<NoImplementationException>(
            () => _mediator.SendAsync<
                AsyncReceiverType, 
                AsyncReceiverResponseType>(
                new AsyncReceiverType("Hello World!")));
    }
    
    [Fact]
    public void SendWithResponseShouldThrowNoImplementationException()
    {
        Assert.Throws<NoImplementationException>(
            () => _mediator.Send<
                ReceiverType, 
                ReceiverResponseType>(
                new ReceiverType("Hello World!")));
    }
}