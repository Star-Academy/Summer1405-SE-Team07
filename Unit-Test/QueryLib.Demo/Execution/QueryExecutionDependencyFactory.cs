using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Connections;
using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.QueryRunners;

namespace QueryLib.Demo.Execution;

public sealed class QueryExecutionDependencyFactory : IQueryExecutionDependencyFactory
{
    private readonly ISqlCompilerFactory _compilerFactory;

    public QueryExecutionDependencyFactory(ISqlCompilerFactory compilerFactory)
    {
        _compilerFactory = compilerFactory ?? throw new ArgumentNullException(nameof(compilerFactory));
    }

    public QueryExecutionDependencies Create(DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return configuration.Provider switch
        {
            DbProvider.PostgreSql => new QueryExecutionDependencies(
                _compilerFactory.Create("postgres"),
                new PostgresQueryRunner(),
                new PostgresConnectionFactory(configuration.ConnectionString)),

            DbProvider.SqlServer => new QueryExecutionDependencies(
                _compilerFactory.Create("sqlserver"),
                new SqlServerQueryRunner(),
                new SqlServerConnectionFactory(configuration.ConnectionString)),

            _ => throw new ArgumentOutOfRangeException(
                nameof(configuration.Provider),
                configuration.Provider,
                "Unsupported database provider."),
        };
    }
}
