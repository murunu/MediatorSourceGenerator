namespace Mediator.SourceGenerator.Helpers;

internal class ItemToGenerate(string className, string interfaceName)
{
    public string ClassName { get; } = className;
    public string InterfaceName { get; } = interfaceName;
}
