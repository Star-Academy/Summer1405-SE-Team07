using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class DatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly IQueryExecutionDependencyFactory _dependencyFactory;
    private readonly ICompiler _compiler;

    public DatabaseQueryExecutor(IQueryExecutionDependencyFactory dependencyFactory,
        ICompiler compiler)
    {
        _dependencyFactory = dependencyFactory ?? throw new ArgumentNullException(nameof(dependencyFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<QueryExecutionResult> ExecuteAsync(Query query, DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(configuration);

        var dependencies = _dependencyFactory.Create(configuration);
        var compiledQuery = _compiler.Compile(query);

        await using var connection = dependencies.Connection;
        await connection.OpenAsync();

        var queryResult = await _runner.RunAsync(compiledQuery, connection);
        return new QueryExecutionResult(compiledQuery, queryResult);
    }
}
