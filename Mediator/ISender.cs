using Mediator.Interfaces;

namespace Mediator;

public interface ISender
{
    /// <summary>
    /// Sends a message of type T.
    /// </summary>
    /// <param name="message">The message to be sent.</param>
    /// <typeparam name="T">The type of the message.</typeparam>
    MediatorResult Send<T>(T message) where T : IRequest;

    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<MediatorResult> SendAsync<T>(T message) where T : IRequest;
    
    /// <summary>
    /// Sends a message and returns the output.
    /// </summary>
    /// <typeparam name="T">The type of the message to be sent.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <param name="message">The message to be sent.</param>
    /// <returns>The output response.</returns>
    MediatorResult<TOutput> Send<T, TOutput>(T message) where T : IRequest;

    /// <summary>
    /// Sends a message asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="T">The type of the message to send.</typeparam>
    /// <typeparam name="TOutput">The type of the response.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the response.</returns>
    Task<MediatorResult<TOutput>> SendAsync<T, TOutput>(T message) where T : IRequest;
}