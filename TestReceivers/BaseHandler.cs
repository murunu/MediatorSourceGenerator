namespace TestReceivers;

public class BaseHandler(string name)
{
    public string Name { get; } = name;

    public override string ToString()
    {
        return Name;
    }
}