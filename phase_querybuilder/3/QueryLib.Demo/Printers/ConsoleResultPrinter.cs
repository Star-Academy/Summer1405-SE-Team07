using QueryLib.Demo.Interfaces;

namespace QueryLib.Demo.Printers;

public class ConsoleResultPrinter : IResultPrinter
{
    public Task PrintAsync(QueryResult result, string header)
    {
        Console.WriteLine($"--- {header} ---");

        foreach (var row in result.Rows)
        {
            var values = row.Select(pair => $"{pair.Key}={pair.Value ?? "NULL"}");
            Console.WriteLine(string.Join(", ", values));
        }

        Console.WriteLine();
        return Task.CompletedTask;
    }
}
