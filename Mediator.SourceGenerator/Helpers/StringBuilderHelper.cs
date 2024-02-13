using System;
using System.Text;

namespace Mediator.SourceGenerator.Helpers;

public static class StringBuilderHelper
{
    public static StringBuilder Build(string @namespace,
        Action<StringBuilder> childItems)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"namespace {@namespace}");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine();

        stringBuilder.AddClass("ConfigureMediator", 1, childItems);
        
        stringBuilder.AppendLine("}");

        return stringBuilder;
    }

    private static StringBuilder AddClass(this StringBuilder stringBuilder, string className, int indentation,
        Action<StringBuilder> childItems)
    {
        stringBuilder.AppendLine();
        
        stringBuilder.Append($"public static partial class {className}", indentation);
        
        stringBuilder.AppendLine();
        
        stringBuilder.Append("{", indentation);

        stringBuilder.AddMethod("AddSourceGenerator", indentation + 1, childItems);
        
        stringBuilder.Append("}", indentation);
        stringBuilder.AppendLine();

        return stringBuilder;
    }
    
    private static StringBuilder AddMethod(this StringBuilder stringBuilder, string methodName, int indentation,
        Action<StringBuilder> childItems)
    {
        stringBuilder.AppendLine();
        
        stringBuilder.Append($"public static Mediator.MediatorConfiguration {methodName}(this Mediator.MediatorConfiguration mediatorConfiguration)", indentation);
        
        stringBuilder.AppendLine();
        
        stringBuilder.Append("{", indentation);
        stringBuilder.AppendLine();

        childItems(stringBuilder);

        stringBuilder.Append(@"
            if (mediatorConfiguration.Services
                .Any(x => x.ImplementationType == typeof(Mediator.Implementations.DefaultMediatorImplementation)))
            {
                return mediatorConfiguration;
            }
        ");
        stringBuilder.AddScopedService(
            new ItemToGenerate(
                "Mediator.Implementations.DefaultMediatorImplementation",
                "Mediator.Implementations.Interfaces.IMediatorImplementation"
            ), 
            indentation - 1);
        stringBuilder.Append(@"
            return mediatorConfiguration;
        }
");
        stringBuilder.AppendLine();

        return stringBuilder;
    }

    private static StringBuilder Indent(this StringBuilder stringBuilder, int indentation)
    {
        for(var i = 0; i < indentation; i++)
            stringBuilder.Append("\t");

        return stringBuilder;
    }
    
    private static StringBuilder Append(this StringBuilder stringBuilder, string value, int indentation)
    {
        stringBuilder.Indent(indentation);
        stringBuilder.Append(value);

        return stringBuilder;
    }

    internal static StringBuilder AddScopedService(this StringBuilder stringBuilder, ItemToGenerate itemToGenerate, int indentation = 3)
    {
        stringBuilder.Append(
            "Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<", indentation);
        stringBuilder.Append(itemToGenerate.InterfaceName);
            
        stringBuilder.Append(", ");
        stringBuilder.Append(itemToGenerate.ClassName);
        stringBuilder.Append(">(mediatorConfiguration.Services);");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();

        return stringBuilder;
    }
}