using Microsoft.Extensions.DependencyInjection;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.QueryRunners;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class QueryExecutionDependencyFactory : IQueryExecutionDependencyFactory
{
    private readonly IReadOnlyDictionary<DbProvider, ICompiler> _compilers;
    private readonly IServiceProvider _provider;

    public QueryExecutionDependencyFactory(
        IReadOnlyDictionary<DbProvider, ICompiler> compilers,
        IServiceProvider provider)
    {
        _compilers = compilers ?? throw new ArgumentNullException(nameof(compilers));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public QueryExecutionDependencies Create(DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var key = configuration.Provider switch
        {
            DbProvider.PostgreSql => "postgres",
            DbProvider.SqlServer => "sqlserver",
            _ => throw new ArgumentOutOfRangeException(
                nameof(configuration.Provider),
                configuration.Provider,
                "Unsupported database provider."),
        };

        var runner = _provider.GetRequiredKeyedService<IQueryRunner>(key);

        var connectionFactoryProvider = _provider.GetRequiredKeyedService<IDbConnectionFactoryProvider>(key);
        var connection = connectionFactoryProvider.CreateConnection(configuration.ConnectionString);

        if (!_compilers.TryGetValue(configuration.Provider, out var compiler))
        {
            throw new ArgumentOutOfRangeException(
                nameof(configuration.Provider),
                configuration.Provider,
                "Unsupported database provider.");
        }

        return new QueryExecutionDependencies(compiler, runner, connection);
    }
}