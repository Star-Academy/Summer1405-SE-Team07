using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class DatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly IDbConnectionFactoryResolver _connectionFactoryResolver;
    private readonly IQueryRunnerFactory _runnerFactory;
    private readonly ICompiler _compiler;

    public DatabaseQueryExecutor(
        IDbConnectionFactoryResolver connectionFactoryResolver,
        IQueryRunnerFactory runnerFactory,
        ICompiler compiler)
    {
        _connectionFactoryResolver = connectionFactoryResolver ?? throw new ArgumentNullException(nameof(connectionFactoryResolver));
        _runnerFactory = runnerFactory ?? throw new ArgumentNullException(nameof(runnerFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<QueryExecutionResult> ExecuteAsync(Query query, DbConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(configuration);

        var factory = _connectionFactoryResolver.GetFactory(configuration.Provider);
        var runner = _runnerFactory.GetRunner(configuration.Provider);

        await using var connection = factory.Create(configuration.ConnectionString);
        var compiledQuery = _compiler.Compile(query, configuration.Provider);

        await connection.OpenAsync();

        var queryResult = await runner.RunAsync(compiledQuery, connection);
        return new QueryExecutionResult(compiledQuery, queryResult);
    }
}
