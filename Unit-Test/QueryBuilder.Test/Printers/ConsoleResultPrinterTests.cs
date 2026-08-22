using FluentAssertions;
using QueryLib.Demo;
using QueryLib.Demo.Printers;

namespace QueryBuilder.Test.Printers;

public class ConsoleResultPrinterTests
{
    private readonly ConsoleResultPrinter _sut;
    
    public ConsoleResultPrinterTests()
    {
        _sut = new ConsoleResultPrinter();
    }

    [Fact]
    public async Task PrintAsync_ShouldWriteHeaderAndRowValuesWithNullAsNULL_WhenResultHasRows()
    {
        // Arrange
        var result = new QueryResult
        {
            ColumnNames = ["id", "name"],
            Rows =
            [
                new Dictionary<string, object?> { ["id"] = 1, ["name"] = "kourosh" },
                new Dictionary<string, object?> { ["id"] = 2, ["name"] = null }
            ]
        };
        var expected =
            "--- Students ---" + Environment.NewLine +
            "id=1, name=kourosh" + Environment.NewLine +
            "id=2, name=NULL" + Environment.NewLine +
            Environment.NewLine;

        await using var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);

        // Act
        await _sut.PrintAsync(result, "Students");
        Console.SetOut(originalOut);

        // Assert
        writer.ToString().Should().Be(expected);
    }

    [Fact]
    public async Task PrintAsync_ShouldWriteOnlyHeader_WhenResultHasNoRows()
    {
        // Arrange
        var result = new QueryResult { ColumnNames = [], Rows = [] };
        var expected = "--- Empty ---" + Environment.NewLine + Environment.NewLine;

        await using var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);

        // Act
        await _sut.PrintAsync(result, "Empty");
        Console.SetOut(originalOut);

        // Assert
        writer.ToString().Should().Be(expected);
    }
}