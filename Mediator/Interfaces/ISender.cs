namespace Mediator.Interfaces;

public interface ISender
{
    void Send<T>(T message);
}