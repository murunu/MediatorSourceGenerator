using Mediator.Exceptions;
using Mediator.UnitTests.Fixtures;
using TestReceivers.Async;
using TestReceivers.Void;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Mediator.UnitTests;

public class BaseMediatorTest : TestBed<BaseMediatorTestFixture>
{
    private readonly IMediator _mediator;
    private const string Message = "Hello World!";
    
    public BaseMediatorTest(ITestOutputHelper testOutputHelper, BaseMediatorTestFixture fixture)
        : base(testOutputHelper, fixture)
    {
        _mediator = _fixture.GetService<IMediator>(_testOutputHelper);
    }
    
    [Fact]
    public async Task SendAsyncShouldRunSuccessfully()
    {
        await _mediator.SendAsync(new AsyncReceiverType(Message));
    }

    [Fact]
    public async Task SendAsyncShouldRunVoidSuccessfully()
    {
        await _mediator.SendAsync(new ReceiverType(Message));
    }
    
    [Fact]
    public void SendShouldRunSuccessfully()
    {
        _mediator.Send(new ReceiverType(Message));
    }

    [Fact]
    public async Task SendAsyncShouldReturnMessage()
    {
        var response = await _mediator.SendAsync<AsyncReceiverType, AsyncReceiverResponseType>(
            new AsyncReceiverType(Message));

        Assert.NotNull(response);
        Assert.NotNull(response.Value);
        Assert.NotNull(response.Value.Name);
        Assert.NotEmpty(response.Value.Name);
        Assert.Equal(Message, response.Value.Name);
    }
    
    [Fact]
    public async Task SendAsyncShouldReturnMessageWhenVoidReturnType()
    {
        var response = await _mediator.SendAsync<ReceiverType, ReceiverResponseType>(
            new ReceiverType(Message));
        
        Assert.NotNull(response);
        Assert.NotNull(response.Value);
        Assert.NotNull(response.Value.Name);
        Assert.NotEmpty(response.Value.Name);
        Assert.Equal(Message, response.Value.Name);
    }
    
    [Fact]
    public void SendShouldReturnMessage()
    {
        var response = _mediator.Send<ReceiverType, ReceiverResponseType>(
            new ReceiverType(Message));
        
        Assert.NotNull(response);
        Assert.NotNull(response.Value);
        Assert.NotNull(response.Value.Name);
        Assert.NotEmpty(response.Value.Name);
        Assert.Equal(Message, response.Value.Name);
    }

    [Fact]
    public void PublishShouldRunSuccessfully()
    {
        _mediator.Publish(new ReceiverType(Message));
    }
    
    [Fact]
    public async Task PublishAsyncShouldRunSuccessfully()
    {
        await _mediator.PublishAsync(new AsyncReceiverType(Message));
    }

    [Fact]
    public async Task SendAsyncWithInvalidTypeShouldThrowException()
    {
        var result = await _mediator.SendAsync(new UnregisteredReceiverType(Message));
        var exception = Assert.Throws<AggregateException>(result.ThrowIfFailure);
        Assert.IsType<NoServiceException>(exception.InnerException);
    }
    
    [Fact]
    public void SendWithInvalidTypeShouldThrowException()
    {
        var result = _mediator.Send(new UnregisteredReceiverType(Message));
        var exception = Assert.Throws<AggregateException>(result.ThrowIfFailure);
        Assert.IsType<NoServiceException>(exception.InnerException);
    }
    
    [Fact]
    public async Task SendAsyncWithInvalidReturnTypeShouldThrowException()
    {
        var result = await _mediator.SendAsync<UnregisteredReceiverType, int>(
            new UnregisteredReceiverType(Message));
        
        var exception = Assert.Throws<AggregateException>(() => result.ThrowIfFailure());
        Assert.IsType<NoServiceException>(exception.InnerException);
    }
    
    [Fact]
    public void SendWithInvalidReturnTypeShouldThrowException()
    {
        var result = _mediator.Send<UnregisteredReceiverType, int>(
            new UnregisteredReceiverType(Message));
        
        var exception = Assert.Throws<AggregateException>(() => result.ThrowIfFailure());
        Assert.IsType<NoServiceException>(exception.InnerException);
    }
}