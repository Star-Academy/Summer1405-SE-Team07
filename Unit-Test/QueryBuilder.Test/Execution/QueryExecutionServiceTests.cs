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
    private readonly QueryExecutionService _sut;

    public QueryExecutionServiceTests()
    {
        _sut = new QueryExecutionService(_queryExecutor, _reporter);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteAndReportEveryConfiguration_WhenAllExecutionsSucceed()
    {
        // Arrange
        var query = new Query().From("Student");
        var configurations = new[]
        {
            new DbConfiguration(DbProvider.PostgreSql, "postgres-connection"),
            new DbConfiguration(DbProvider.SqlServer, "sql-server-connection"),
        };
        var executionResult = CreateExecutionResult();

        _queryExecutor.ExecuteAsync(query, Arg.Any<DbConfiguration>()).Returns(executionResult);

        // Act
        await _sut.ExecuteAsync(query, configurations);

        // Assert
        await _queryExecutor.Received(1).ExecuteAsync(query, configurations[0]);
        await _queryExecutor.Received(1).ExecuteAsync(query, configurations[1]);
        await _reporter.Received(1).ReportSucceededAsync(DbProvider.PostgreSql, executionResult);
        await _reporter.Received(1).ReportSucceededAsync(DbProvider.SqlServer, executionResult);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReportFailureAndContinue_WhenAnExecutionFails()
    {
        // Arrange
        var query = new Query().From("Student");
        var failedConfiguration = new DbConfiguration(DbProvider.PostgreSql, "postgres-connection");
        var successfulConfiguration = new DbConfiguration(DbProvider.SqlServer, "sql-server-connection");
        var configurations = new[]
        {
            failedConfiguration,
            successfulConfiguration,
        };
        var exception = new InvalidOperationException("Database unavailable");
        var executionResult = CreateExecutionResult();

        _queryExecutor.ExecuteAsync(query, failedConfiguration).Returns<Task<QueryExecutionResult>>(_ => throw exception);
        _queryExecutor.ExecuteAsync(query, successfulConfiguration).Returns(executionResult);

        // Act
        await _sut.ExecuteAsync(query, configurations);

        // Assert
        _reporter.Received(1).ReportFailed(DbProvider.PostgreSql, exception);
        await _reporter.Received(1).ReportSucceededAsync(DbProvider.SqlServer, executionResult);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReportCompletionForEveryConfiguration_WhenExecutionsFinish()
    {
        // Arrange
        var query = new Query().From("Student");
        var configurations = new[]
        {
            new DbConfiguration(DbProvider.PostgreSql, "postgres-connection"),
            new DbConfiguration(DbProvider.SqlServer, "sql-server-connection"),
        };
        var executionResult = CreateExecutionResult();

        _queryExecutor.ExecuteAsync(query, Arg.Any<DbConfiguration>()).Returns(executionResult);

        // Act
        await _sut.ExecuteAsync(query, configurations);

        // Assert
        _reporter.Received(configurations.Length).ReportCompleted();
    }

    private static QueryExecutionResult CreateExecutionResult()
    {
        return new QueryExecutionResult(
            new CompiledQuery("SELECT 1", []),
            new QueryResult());
    }
}
