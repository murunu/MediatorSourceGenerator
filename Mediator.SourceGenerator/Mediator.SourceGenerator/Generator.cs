using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Mediator.SourceGenerator.Helpers;
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
        
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;
        
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        // Generate the source using the compilation and classes
        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        if(context.Node is not ClassDeclarationSyntax classDeclarationSyntax) return null;

        // loop through all the attributes on the method
        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the [RegisterMediatorAttribute] attribute?
                if (fullName == "Mediator.RegisterMediatorAttribute")
                {
                    // return the class
                    return classDeclarationSyntax;
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    private static void Execute(
        Compilation compilation, 
        ImmutableArray<ClassDeclarationSyntax> items, 
        SourceProductionContext context)
    {
        if (items.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var distinctItems = items.Distinct();

        var itemsToGenerate = GetTypesToGenerate(compilation, distinctItems, context.CancellationToken);

        if (itemsToGenerate.Count <= 0)
        {
            return;
        }
        
        // generate the source code and add it to the output
        var result = GenerateExtensionClass(itemsToGenerate);
        context.AddSource("Mediator.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    private static List<ItemToGenerate> GetTypesToGenerate(
        Compilation compilation, 
        IEnumerable<ClassDeclarationSyntax> classes, 
        CancellationToken ct)
    {
        // Create a list to hold our output
        List<ItemToGenerate> itemsToGenerate = [];
        
        // Get the semantic representation of our attribute 
        var itemAttribute = compilation.GetTypeByMetadataName("Mediator.RegisterMediatorAttribute");
        if (itemAttribute == null)
        {
            // If this is null, the compilation couldn't find the attribute type
            // which suggests there's something very wrong! Bail out..
            return itemsToGenerate;
        }

        itemsToGenerate.AddRange(
            classes
                .Select(classDeclarationSyntax => 
                    GenerateItem(compilation, ct, classDeclarationSyntax))
                .OfType<ItemToGenerate>());

        return itemsToGenerate;
    }
    
    private static ItemToGenerate? GenerateItem(
        Compilation compilation, 
        CancellationToken ct, 
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        // stop if we're asked to
        ct.ThrowIfCancellationRequested();
    
        // Get the semantic representation of the syntax
        var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
    
        if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol itemSymbol)
            return null;
    
        var @interface = itemSymbol.Interfaces.FirstOrDefault();
    
        if (@interface == null)
           return null;
    
        var itemName = itemSymbol.ToString();
    
        return new ItemToGenerate(itemName, @interface.ToString());
    }
    
    private static string GenerateExtensionClass(List<ItemToGenerate> itemsToGenerate)
    {
        var result = StringBuilderHelper.Build("Mediator", x =>
        {
            foreach (var member in itemsToGenerate)
            {
                x.AddScopedService(member);
            }

            x.TryAddScopedService(new ItemToGenerate(
                "Mediator.MediatorBase", 
                "Mediator.IMediator"));
        });
        
        return result.ToString();
    }
}