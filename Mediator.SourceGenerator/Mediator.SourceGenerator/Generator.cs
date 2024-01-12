using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mediator.SourceGenerator;

[Generator]
public class ServicesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterPostInitializationOutput(context => context.AddSource(
            "MediatorServicesExtensions.g.cs",
            SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)
            ));
        
        IncrementalValuesProvider<ClassDeclarationSyntax> enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
            .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

        // Combine the selected enums with the `Compilation`
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndEnums
            = context.CompilationProvider.Combine(enumDeclarations.Collect());

        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(compilationAndEnums,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }
    
    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    
    static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        if(context.Node is not ClassDeclarationSyntax enumDeclarationSyntax) return null;

        // loop through all the attributes on the method
        foreach (AttributeListSyntax attributeListSyntax in enumDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the [RegisterMediatorAttribute] attribute?
                if (fullName == "Mediator.RegisterMediatorAttribute")
                {
                    // return the enum
                    return enumDeclarationSyntax;
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }  
    
    static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> enums, SourceProductionContext context)
    {
        if (enums.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
        IEnumerable<ClassDeclarationSyntax> distinctEnums = enums.Distinct();

        // Convert each EnumDeclarationSyntax to an EnumToGenerate
        var enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

        // If there were errors in the EnumDeclarationSyntax, we won't create an
        // EnumToGenerate for it, so make sure we have something to generate
        if (enumsToGenerate.Count > 0)
        {
            // generate the source code and add it to the output
            string result = GenerateExtensionClass(enumsToGenerate);
            context.AddSource("Mediator.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }
    
    static List<ItemToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, CancellationToken ct)
    {
        // Create a list to hold our output
        var enumsToGenerate = new List<ItemToGenerate>();
        // Get the semantic representation of our marker attribute 
        INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName("Mediator.RegisterMediatorAttribute");

        if (enumAttribute == null)
        {
            // If this is null, the compilation couldn't find the marker attribute type
            // which suggests there's something very wrong! Bail out..
            return [];
        }

        foreach (ClassDeclarationSyntax enumDeclarationSyntax in classes)
        {
            // stop if we're asked to
            ct.ThrowIfCancellationRequested();

            // Get the semantic representation of the enum syntax
            SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
            {
                // something went wrong, bail out
                continue;
            }

            var @interface = enumSymbol.Interfaces.FirstOrDefault();

            var genericTypes = @interface.TypeArguments;
            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            string enumName = enumSymbol.ToString();
            

            // Create an EnumToGenerate for use in the generation phase
            enumsToGenerate.Add(new ItemToGenerate
            {
                ClassName = enumName,
                IsAsync = @interface.ToString().ToLower().Contains("async"),
                HasResponse = genericTypes.Length > 1,
                InputType = genericTypes.Length > 0 ? genericTypes[0].ToString() : string.Empty,
                ResponseType = genericTypes.Length > 1 ? genericTypes[1].ToString() : string.Empty
            });
        }

        return enumsToGenerate;
    }
    
    public static string GenerateExtensionClass(List<ItemToGenerate> enumsToGenerate)
    {
        var sb = new StringBuilder();
        sb.Append(@"
namespace Mediator
{
    public static partial class ConfigureMediator
    {
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddMediator(
            this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            var mediatorConfiguration = new Mediator.MediatorOptions();

");
        foreach (var member in enumsToGenerate)
        {
            sb.Append("            mediatorConfiguration.Receivers.Add(new Mediator.ReceiverType(){");

            sb.Append($@"
                Type = typeof({member.ClassName}),
                InputType = typeof({member.InputType}),
                IsAsync = {member.IsAsync.ToString().ToLower()},
                HasResponse = {member.HasResponse.ToString().ToLower()},");
            if (member.HasResponse)
            {
                sb.AppendLine();
                sb.Append($"                ResponseType = typeof({member.ResponseType})");
            }

            sb.Append(@"
            });
");
           
            sb.AppendLine();
            
            sb.Append(
                "            Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddScoped<");
            sb.Append(member.ClassName);
            sb.Append(">(services);");
            sb.AppendLine();
            sb.AppendLine();
            //
            // sb.AppendLine($"Classname: {member.ClassName}");
            // sb.AppendLine($"InputType: {member.InputType}");
            // sb.AppendLine($"ResponseType: {member.ResponseType}");
            // sb.AppendLine($"IsAsync: {member.IsAsync}");
            // sb.AppendLine($"HasResponse: {member.HasResponse}");
        }
        sb.Append(@"
            Microsoft.Extensions.DependencyInjection.OptionsServiceCollectionExtensions.Configure<Mediator.MediatorOptions>(services, 
            options =>
            {
                options.Receivers = mediatorConfiguration.Receivers;
            });

            Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddScoped<Mediator.IMediator, Mediator.MediatorBase>(services);

            return services;
        }");
        sb.Append(@"
    }
}");

        return sb.ToString();
    }
}

public static class SourceGenerationHelper
{
    public const string Attribute = @"
using System;

namespace Mediator
{
    public class RegisterMediatorAttribute : Attribute
    {
    }
}";
}

public class ItemToGenerate
{
    public string ClassName { get; set; }
    public string InputType { get; set; }
    public bool IsAsync { get; set; }
    public bool HasResponse { get; set; }
    public string ResponseType { get; set; }
}
