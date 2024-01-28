using Mediator.Interfaces;

namespace TestReceivers;

public class BaseHandler(string name) : IRequest
{
    public string Name { get; } = name;

    public override string ToString()
    {
        return Name;
    }
}