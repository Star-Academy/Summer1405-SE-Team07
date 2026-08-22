using FluentAssertions;
using NSubstitute;
using QueryLib.Compilers;
using QueryLib.Demo;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution;

namespace QueryBuilder.Test.Execution;

public class ConsoleQueryExecutionReporterTests
{
    private readonly IResultPrinter _resultPrinter = Substitute.For<IResultPrinter>();
    private readonly ConsoleQueryExecutionReporter _sut;

    public ConsoleQueryExecutionReporterTests()
    {
        _sut = new ConsoleQueryExecutionReporter(_resultPrinter);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenResultPrinterIsNull()
    {
        // Arrange

        // Act
        var act = () => new ConsoleQueryExecutionReporter(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("resultPrinter");
    }

    [Fact]
    public async Task ReportSucceededAsync_ShouldThrowArgumentNullException_WhenExecutionResultIsNull()
    {
        // Arrange

        // Act
        var act = () => _sut.ReportSucceededAsync(DbProvider.PostgreSql, null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("executionResult");
    }

    [Fact]
    public async Task ReportSucceededAsync_ShouldPrintResultWithProviderHeader_WhenExecutionResultIsProvided()
    {
        // Arrange
        var queryResult = new QueryResult { ColumnNames = ["id"], Rows = [] };
        var executionResult = new QueryExecutionResult(new CompiledQuery("SELECT 1", []), queryResult);
        var expected = ("PostgreSql Results", queryResult);

        // Act
        await _sut.ReportSucceededAsync(DbProvider.PostgreSql, executionResult);

        // Assert
        await _resultPrinter.Received(1).PrintAsync(expected.Item2, expected.Item1);
    }

    [Fact]
    public void ReportFailed_ShouldThrowArgumentNullException_WhenExceptionIsNull()
    {
        // Arrange

        // Act
        var act = () => _sut.ReportFailed(DbProvider.SqlServer, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("exception");
    }

    [Fact]
    public void ReportStarted_ShouldNotThrow_WhenCalled()
    {
        // Arrange
        
        // Act
        var act = () => _sut.ReportStarted(DbProvider.PostgreSql);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ReportCompleted_ShouldNotThrow_WhenCalled()
    {
        // Arrange
        
        // Act
        var act = () => _sut.ReportCompleted();

        // Assert
        act.Should().NotThrow();
    }
}