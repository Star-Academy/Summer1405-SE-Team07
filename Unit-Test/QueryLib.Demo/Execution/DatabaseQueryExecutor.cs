using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class DatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly IQueryExecutionDependencyFactory _dependencyFactory;

    public DatabaseQueryExecutor(IQueryExecutionDependencyFactory dependencyFactory)
    {
        _dependencyFactory = dependencyFactory ?? throw new ArgumentNullException(nameof(dependencyFactory));
    }

    public async Task<QueryExecutionResult> ExecuteAsync(Query query, DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(configuration);

        var dependencies = _dependencyFactory.Create(configuration);
        var compiledQuery = dependencies.Compiler.Compile(query);

        await using var connection = dependencies.Connection;
        await connection.OpenAsync();

        var queryResult = await dependencies.Runner.RunAsync(compiledQuery, connection);
        return new QueryExecutionResult(compiledQuery, queryResult);
    }
}
