using Microsoft.Extensions.DependencyInjection;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Extensions;
using Xunit;

namespace QueryLib.IntegrationTest.Infrastructure;

public sealed class CompilerFixture : IAsyncLifetime
{
    private ServiceProvider? _serviceProvider;

    public ICompiler Compiler => _serviceProvider!.GetRequiredService<ICompiler>();

    public Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddQueryLibServices();
        _serviceProvider = services.BuildServiceProvider();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
        }
    }
}
