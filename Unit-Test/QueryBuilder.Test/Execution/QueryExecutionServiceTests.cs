using FluentAssertions;
using NSubstitute;
using QueryLib;
using QueryLib.Compilers;
using QueryLib.Demo;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryBuilder.Test.Execution;

public class QueryExecutionServiceTests
{
    private readonly IDatabaseQueryExecutor _queryExecutor = Substitute.For<IDatabaseQueryExecutor>();
    private readonly IQueryExecutionReporter _reporter = Substitute.For<IQueryExecutionReporter>();
    private readonly QueryExecutionService _sut;

    public QueryExecutionServiceTests()
    {
        _sut = new QueryExecutionService(_queryExecutor, _reporter);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExecutorIsNull()
    {
        // Arrange
        var act = () => new QueryExecutionService(null!, _reporter);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("queryExecutor");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenReporterIsNull()
    {
        // Arrange
        var act = () => new QueryExecutionService(_queryExecutor, null!);

        // Act
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("reporter");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var act = () => _sut.ExecuteAsync(null!, []);

        // Act
        
        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("query");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenConfigurationsIsNull()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        var act = () => _sut.ExecuteAsync(query, null!);

        // Act
        
        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("configurations");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReportStartedSucceededAndCompleted_WhenExecutionSucceeds()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "conn");
        var executionResult = new QueryExecutionResult(
            new CompiledQuery("SELECT 1", []),
            new QueryResult { ColumnNames = [], Rows = [] });
        _queryExecutor.ExecuteAsync(query, configuration).Returns(Task.FromResult(executionResult));

        // Act
        await _sut.ExecuteAsync(query, [configuration]);

        // Assert
        Received.InOrder(() =>
        {
            _reporter.ReportStarted(DbProvider.PostgreSql);
            _reporter.ReportSucceededAsync(DbProvider.PostgreSql, executionResult);
            _reporter.ReportCompleted();
        });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReportFailedAndCompleted_WhenExecutionThrows()
    {
        // Arrange
        var query = new Query().From("student").Select("id");
        var configuration = new DbConfiguration(DbProvider.PostgreSql, "conn");
        var thrownException = new InvalidOperationException("boom");
        _queryExecutor.ExecuteAsync(query, configuration)
            .Returns(Task.FromException<QueryExecutionResult>(thrownException));

        // Act
        await _sut.ExecuteAsync(query, [configuration]);

        // Assert
        Received.InOrder(() =>
        {
            _reporter.ReportStarted(DbProvider.PostgreSql);
            _reporter.ReportFailed(DbProvider.PostgreSql, thrownException);
            _reporter.ReportCompleted();
        });
    }
}