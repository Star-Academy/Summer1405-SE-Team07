using QueryLib.Compilers;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.Printers;
using FluentAssertions;
using QueryLib.Demo.Execution;
using QueryLib.Demo;
using NSubstitute;

namespace QueryBuilder.Test.Execution;

public class ConsoleQueryExecutionReporterTests
{
    private readonly IResultPrinter _resultPrinter;
    private readonly IQueryExecutionReporter _sut;

    public ConsoleQueryExecutionReporterTests()
    {
        _resultPrinter = Substitute.For<IResultPrinter>();
        _sut = new ConsoleQueryExecutionReporter(_resultPrinter);
    }
    [Fact]
    public void Constructor_WhenResultPrinterIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IResultPrinter? resultPrinter = null;

        // Act
        var act = () => new ConsoleQueryExecutionReporter(resultPrinter!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("resultPrinter");
    }

    [Fact]
    public void ReportStarted_WhenProviderIsSupplied_ShouldWriteProviderHeaderToConsole()
    {
        // Arrange
        using var writer = new StringWriter();
        var originalOutput = Console.Out;
        Console.SetOut(writer);

        try
        {
            // Act
            _sut.ReportStarted(DbProvider.PostgreSql);

            // Assert
            writer.ToString().Should().Be("========== PostgreSql ==========" + Environment.NewLine);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    [Fact]
    public void ReportCompleted_WhenCalled_ShouldWriteBlankLineToConsole()
    {
        // Arrange
        using var writer = new StringWriter();
        var originalOutput = Console.Out;
        Console.SetOut(writer);

        try
        {
            // Act
            _sut.ReportCompleted();

            // Assert
            writer.ToString().Should().Be(Environment.NewLine);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    [Fact]
    public async Task ReportSucceededAsync_WhenExecutionResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        QueryExecutionResult? executionResult = null;

        // Act
        var act = () => _sut.ReportSucceededAsync(DbProvider.PostgreSql, executionResult!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("executionResult");
    }

    [Fact]
    public async Task ReportSucceededAsync_WhenExecutionResultIsValid_ShouldPrintQueryResult()
    {
        // Arrange
        using var writer = new StringWriter();
        var originalOutput = Console.Out;
        Console.SetOut(writer);
        var provider = DbProvider.PostgreSql;
        var compiledQuery = new CompiledQuery(
            "SELECT \"id\" FROM \"student\"",
            new List<object?> { 10 });
        var queryResult = new QueryResult();
        var queryExecutionResult = new QueryExecutionResult(
            compiledQuery,
            queryResult);

        try
        {
            // Act
            await _sut.ReportSucceededAsync(provider, queryExecutionResult);

            // Assert
            await _resultPrinter.Received(1).PrintAsync(
                queryResult,
                "PostgreSql Results");
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    [Fact]
    public void ReportFailed_WhenExceptionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception? exception = null;

        // Act
        var act = () => _sut.ReportFailed(DbProvider.SqlServer, exception!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("exception");
    }

    [Fact]
    public void ReportFailed_WhenExceptionIsValid_ShouldWriteErrorToConsole()
    {
        // Arrange
        var provider = DbProvider.PostgreSql;
        var exception = new Exception("Connection failed");

        using var writer = new StringWriter();
        var originalError = Console.Error;

        Console.SetError(writer);

        try
        {
            // Act
            _sut.ReportFailed(provider, exception);

            // Assert
            writer.ToString()
                .Trim()
                .Should()
                .Be("Error executing query on PostgreSql: Connection failed");
        }
        finally
        {
            Console.SetError(originalError);
        }
    }
}

