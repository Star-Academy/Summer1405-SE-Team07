using FluentAssertions;
using QueryLib.Demo.Printers;
using QueryLib.Demo;

namespace QueryBuilder.Test.Printers;

public class ConsoleResultPrinterTests
{
    [Fact]
    public async Task PrintAsync_WhenRowsAreEmpty_ShouldComplete()
    {
        // Arrange
        var printer = new ConsoleResultPrinter();
        var result = new QueryResult();

        // Act
        var act = () => printer.PrintAsync(result, "Test");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PrintAsync_WhenRowContainsValues_ShouldPrintSuccessfully()
    {
        // Arrange
        var printer = new ConsoleResultPrinter();
        var result = new QueryResult();

        result.AddRow(new Dictionary<string, object?>
        {
            ["Name"] = "Kourosh",
            ["Age"] = 23,
            ["Address"] = null
        });

        // Act
        var act = () => printer.PrintAsync(result, "Test");

        // Assert
        await act.Should().NotThrowAsync();
    }
}