using Microsoft.Extensions.DependencyInjection;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Connections;
using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.QueryRunners;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class QueryExecutionDependencyFactory : IQueryExecutionDependencyFactory
{
    private readonly ISqlCompilerFactory _compilerFactory;
    private readonly IServiceProvider _provider;

    public QueryExecutionDependencyFactory(ISqlCompilerFactory compilerFactory, IServiceProvider provider)
    {
        _compilerFactory = compilerFactory ?? throw new ArgumentNullException(nameof(compilerFactory));
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
        var connectionFactoryBuilder = _provider.GetRequiredKeyedService<Func<string, IDbConnectionFactory>>(key);
        var connectionFactory = connectionFactoryBuilder(configuration.ConnectionString);

        return new QueryExecutionDependencies(
            _compilerFactory.Create(key),
            runner,
            connectionFactory);
    }
}