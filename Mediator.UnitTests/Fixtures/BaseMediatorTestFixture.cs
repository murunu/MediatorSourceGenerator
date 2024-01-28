using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Mediator.UnitTests.Fixtures;


public class BaseMediatorTestFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddMediator()
            .AddDefaultMediator()
            .AddSourceGenerator();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        return Array.Empty<TestAppSettings>();
    }

    [ExcludeFromCodeCoverage]
    protected override ValueTask DisposeAsyncCore()
    {
        return new ValueTask();
    }
}