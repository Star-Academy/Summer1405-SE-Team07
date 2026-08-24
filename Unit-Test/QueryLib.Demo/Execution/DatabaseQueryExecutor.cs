using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class DatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly IReadOnlyDictionary<DbProvider, IDbConnectionFactory> _connectionFactories;
    private readonly IReadOnlyDictionary<DbProvider, IQueryRunner> _runners;
    private readonly ICompiler _compiler;

    public DatabaseQueryExecutor(
        IReadOnlyDictionary<DbProvider, IDbConnectionFactory> connectionFactories,
        IReadOnlyDictionary<DbProvider, IQueryRunner> runners,
        ICompiler compiler)
    {
        _connectionFactories = connectionFactories ?? throw new ArgumentNullException(nameof(connectionFactories));
        _runners = runners ?? throw new ArgumentNullException(nameof(runners));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<QueryExecutionResult> ExecuteAsync(Query query, DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(configuration);

        if (!_connectionFactories.TryGetValue(configuration.Provider, out var factory))
        {
            throw new ArgumentOutOfRangeException(
                nameof(configuration.Provider),
                configuration.Provider,
                "Unsupported database provider.");
        }

        if (!_runners.TryGetValue(configuration.Provider, out var runner))
        {
            throw new ArgumentOutOfRangeException(
                nameof(configuration.Provider),
                configuration.Provider,
                "Unsupported database provider.");
        }

        await using var connection = factory.Create(configuration.ConnectionString);
        var compiledQuery = _compiler.Compile(query, configuration.Provider);

        await connection.OpenAsync();

        var queryResult = await runner.RunAsync(compiledQuery, connection);
        return new QueryExecutionResult(compiledQuery, queryResult);
    }
}
