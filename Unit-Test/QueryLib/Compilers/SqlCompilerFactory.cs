using QueryLib.Compilers.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace QueryLib.Compilers;

public class SqlCompilerFactory : ISqlCompilerFactory
{
    private readonly IServiceProvider _provider;

    public SqlCompilerFactory(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public ICompiler Create(string compilerType) =>
        _provider.GetRequiredKeyedService<ICompiler>(compilerType);
}