using Mediator.Interfaces;

namespace Mediator;

public interface IPublisher
{
    /// <summary>
    /// Publishes a message of type T.
    /// </summary>
    /// <typeparam name="T">The type of the message to be published.</typeparam>
    /// <param name="message">The message to be published.</param>
    MediatorResult Publish<T>(T message) where T : IRequest;

    /// <summary>
    /// Publishes the specified message asynchronously.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<MediatorResult> PublishAsync<T>(T message) where T : IRequest;
}