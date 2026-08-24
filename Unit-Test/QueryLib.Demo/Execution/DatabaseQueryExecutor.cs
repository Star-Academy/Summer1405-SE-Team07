using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class DatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly IReadOnlyDictionary<DbProvider, IDbConnectionFactory> _connectionFactories;
    private readonly ICompiler _compiler;
    private readonly IQueryRunner _runner;

    public DatabaseQueryExecutor(
        IReadOnlyDictionary<DbProvider, IDbConnectionFactory> connectionFactories,
        ICompiler compiler,
        IQueryRunner runner)
    {
        _connectionFactories = connectionFactories ?? throw new ArgumentNullException(nameof(connectionFactories));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
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

        await using var connection = factory.Create(configuration.ConnectionString);
        var compiledQuery = _compiler.Compile(query, configuration.Provider);

        await connection.OpenAsync();

        var queryResult = await _runner.RunAsync(compiledQuery, connection);
        return new QueryExecutionResult(compiledQuery, queryResult);
    }
}
