namespace Mediator;

public class MediatorOptions
{
    public List<ReceiverType> Receivers { get; set; } = [];
}

public class ReceiverType
{
    public Type Type { get; set; }
    public Type InputType { get; set; }
    public bool IsAsync { get; set; }
    public bool HasResponse { get; set; }
    public Type? ResponseType { get; set; }
}