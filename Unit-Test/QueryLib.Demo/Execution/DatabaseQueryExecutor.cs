using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class DatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly IQueryExecutionDependencyFactory _dependencyFactory;
    private readonly ICompiler _compiler;
    private readonly IQueryRunner _runner;

    public DatabaseQueryExecutor(
        IQueryExecutionDependencyFactory dependencyFactory,
        ICompiler compiler,
        IQueryRunner runner)
    {
        _dependencyFactory = dependencyFactory ?? throw new ArgumentNullException(nameof(dependencyFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
    }

    public async Task<QueryExecutionResult> ExecuteAsync(Query query, DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(configuration);

        await using var connection = _dependencyFactory.Create(configuration);
        var compiledQuery = _compiler.Compile(query);

        await connection.OpenAsync();

        var queryResult = await _runner.RunAsync(compiledQuery, connection);
        return new QueryExecutionResult(compiledQuery, queryResult);
    }
}
