using NSubstitute;
using QueryLib;
using QueryLib.Compilers;
using QueryLib.Demo;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryBuilder.Test.Execution;

public sealed class QueryExecutionServiceTests
{
    private readonly IDatabaseQueryExecutor _queryExecutor = Substitute.For<IDatabaseQueryExecutor>();
    private readonly IQueryExecutionReporter _reporter = Substitute.For<IQueryExecutionReporter>();

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteAndReportEveryConfiguration()
    {
        var query = new Query().From("Student");
        var configurations = new[]
        {
            new DbConfiguration(DbProvider.PostgreSql, "postgres-connection"),
            new DbConfiguration(DbProvider.SqlServer, "sql-server-connection"),
        };
        var executionResult = CreateExecutionResult();
        _queryExecutor.ExecuteAsync(query, Arg.Any<DbConfiguration>()).Returns(executionResult);
        var service = new QueryExecutionService(_queryExecutor, _reporter);

        await service.ExecuteAsync(query, configurations);

        await _queryExecutor.Received(1).ExecuteAsync(query, configurations[0]);
        await _queryExecutor.Received(1).ExecuteAsync(query, configurations[1]);
        await _reporter.Received(1).ReportSucceededAsync(DbProvider.PostgreSql, executionResult);
        await _reporter.Received(1).ReportSucceededAsync(DbProvider.SqlServer, executionResult);
        _reporter.Received(2).ReportCompleted();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldContinueWithNextConfiguration_WhenExecutionFails()
    {
        var query = new Query().From("Student");
        var failedConfiguration = new DbConfiguration(DbProvider.PostgreSql, "postgres-connection");
        var successfulConfiguration = new DbConfiguration(DbProvider.SqlServer, "sql-server-connection");
        var exception = new InvalidOperationException("Database unavailable");
        var executionResult = CreateExecutionResult();
        _queryExecutor.ExecuteAsync(query, failedConfiguration).Returns<Task<QueryExecutionResult>>(_ => throw exception);
        _queryExecutor.ExecuteAsync(query, successfulConfiguration).Returns(executionResult);
        var service = new QueryExecutionService(_queryExecutor, _reporter);

        await service.ExecuteAsync(query, [failedConfiguration, successfulConfiguration]);

        _reporter.Received(1).ReportFailed(DbProvider.PostgreSql, exception);
        await _queryExecutor.Received(1).ExecuteAsync(query, successfulConfiguration);
        await _reporter.Received(1).ReportSucceededAsync(DbProvider.SqlServer, executionResult);
        _reporter.Received(2).ReportCompleted();
    }

    private static QueryExecutionResult CreateExecutionResult()
    {
        return new QueryExecutionResult(
            new CompiledQuery("SELECT 1", []),
            new QueryResult());
    }
}
