namespace Mediator.SourceGenerator.Helpers;

internal class ItemToGenerate(string className, string interfaceName)
{
    public string ClassName { get; set; } = className;
    public string InterfaceName { get; set; } = interfaceName;
}
