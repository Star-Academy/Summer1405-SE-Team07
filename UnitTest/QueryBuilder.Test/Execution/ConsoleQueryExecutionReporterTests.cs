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
    public void Constructor_ShouldThrowArgumentNullException_WhenResultPrinterIsNull()
    {
        // Arrange 
        var act = () => new ConsoleQueryExecutionReporter(null!);
        // Act & Assert
        
        act.Should().Throw<ArgumentNullException>().WithParameterName("resultPrinter");
    }
    
    [Fact]
    public void ReportStarted_ShouldWriteProviderToConsole()
    {
        // Arrange
        using var writer = new StringWriter();
        Console.SetOut(writer);
        // Act
        _sut.ReportStarted(DbProvider.PostgreSql);
        // Assert
        writer.ToString().Should().Be("========== PostgreSql ==========" + Environment.NewLine);
    }
    
    
    [Fact]
    public void ReportCompleted_ShouldWriteALine()
    {
        // Arrange
        using var writer = new StringWriter();
        Console.SetOut(writer);
        // Act
        _sut.ReportCompleted();
        // Assert
        writer.ToString().Should().Be("" + Environment.NewLine);
    }

    [Fact]
    public async Task ReportSucceededAsync_ShouldThrowArgumentNullException_WhenExecutionResultIsNull()
    {
        // Arrange 
        var act = () => _sut.ReportSucceededAsync(DbProvider.PostgreSql, null!);
        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("executionResult");
    }

    [Fact]
    public async Task ReportSucceededAsync_ShouldPrintResult_WhenExecutionResultIsValid()
    {
        using var writer = new StringWriter();
        Console.SetOut(writer);
        
        // Arrange
        var provider = DbProvider.PostgreSql;

        var compiledQuery = new CompiledQuery(
            "SELECT \"id\" FROM \"student\"",
            new List<object?> { 10 });


        var queryResult = new QueryResult();

        var queryExecutionResult = new QueryExecutionResult(
            compiledQuery,
            queryResult);

        await _sut.ReportSucceededAsync(provider, queryExecutionResult);

        await _resultPrinter.Received(1).PrintAsync(
            queryResult,
            "PostgreSql Results");
    }

    [Fact]
    public void reportfailed_ShouldThrowArgumentNullException_WhenExecutionResultIsNull()
    {
        var act = () => _sut.ReportFailed(DbProvider.SqlServer, null!);
        
        act.Should().Throw<ArgumentNullException>().WithParameterName("exception");
    }

   
    [Fact]
    public void ReportFailed_ShouldWriteErrorToConsole_WhenExceptionIsValid()
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

