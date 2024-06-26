using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mediator.SourceGenerator.Tests;

public class MediatorSourceGeneratorTest
{
    public const string ReceiverClassText = @"
using Mediator;

namespace TestReceivers;

[RegisterMediator]
public class Receiver : Mediator.Interfaces.IReceiver<string>
{
    public void Receive(string message)
    {
        // Handle the received message
        Console.WriteLine($""Received at ConcreteReceiver message: {message}"");
    }
}

[RegisterMediator]
public class AsyncReceiver : Mediator.Interfaces.IAsyncReceiver<string>
{
    public async Task ReceiveAsync(string message)
    {
        // Handle the received message
        Console.WriteLine($""Received at ConcreteReceiver message: {message}"");
    }
}

[RegisterMediator]
public class ReceiverWithResponse : Mediator.Interfaces.IReceiver<string, string>
{
    public void Receive(string message)
    {
        // Handle the received message
        Console.WriteLine($""Received at ConcreteReceiver message: {message}"");
    }
}

[RegisterMediator]
public class AsyncReceiverWithResponse : Mediator.Interfaces.IAsyncReceiver<string, string>
{
    public async Task ReceiveAsync(string message)
    {
        // Handle the received message
        Console.WriteLine($""Received at ConcreteReceiver message: {message}"");
    }
}
";

    public const string ExpectedGeneratedClassText = @"namespace Mediator
{


	public static partial class ConfigureMediator
	{
		public static Mediator.MediatorConfiguration AddSourceGenerator(this Mediator.MediatorConfiguration mediatorConfiguration)
		{
			Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<Mediator.Interfaces.IReceiver<string>, TestReceivers.Receiver>(mediatorConfiguration.Services);

			Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<Mediator.Interfaces.IAsyncReceiver<string>, TestReceivers.AsyncReceiver>(mediatorConfiguration.Services);

			Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<Mediator.Interfaces.IReceiver<string, string>, TestReceivers.ReceiverWithResponse>(mediatorConfiguration.Services);

			Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<Mediator.Interfaces.IAsyncReceiver<string, string>, TestReceivers.AsyncReceiverWithResponse>(mediatorConfiguration.Services);


            if (mediatorConfiguration.Services
                .Any(x => x.ImplementationType == typeof(Mediator.Implementations.DefaultMediatorImplementation)))
            {
                return mediatorConfiguration;
            }
        	Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped<Mediator.Implementations.Interfaces.IMediatorImplementation, Mediator.Implementations.DefaultMediatorImplementation>(mediatorConfiguration.Services);


            return mediatorConfiguration;
        }

	}
}
";
    
    [Fact]
    public void GenerateServicesMethod()
    {
        // Create an instance of the source generator.
        var generator = new ServicesGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(generator);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(nameof(MediatorSourceGeneratorTest),
            new[] { CSharpSyntaxTree.ParseText(ReceiverClassText) },
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IMediator).Assembly.Location)
            });

        // Run generators and retrieve all results.
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // All generated files can be found in 'RunResults.GeneratedTrees'.
        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("Mediator.g.cs"));

        // Complex generators should be tested using text comparison.
        Assert.Equal(ExpectedGeneratedClassText,
            generatedFileSyntax.GetText().ToString(),
            ignoreLineEndingDifferences: true);
    }
}