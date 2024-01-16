using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Mediator.UnitTests.Fixtures;

public class NoImplementationMediatorTestFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddMediator();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        return Array.Empty<TestAppSettings>();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        return new ValueTask();
    }
}