using QueryLib.Compilers;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class ConsoleQueryExecutionReporter : IQueryExecutionReporter
{
    private readonly IResultPrinter _resultPrinter;

    public ConsoleQueryExecutionReporter(IResultPrinter resultPrinter)
    {
        _resultPrinter = resultPrinter ?? throw new ArgumentNullException(nameof(resultPrinter));
    }

    public void ReportStarted(DbProvider provider)
    {
        Console.WriteLine($"========== {provider} ==========");
    }

    public async Task ReportSucceededAsync(DbProvider provider, QueryExecutionResult executionResult)
    {
        ArgumentNullException.ThrowIfNull(executionResult);

        PrintCompiledQuery(provider, executionResult.CompiledQuery);
        await _resultPrinter.PrintAsync(executionResult.QueryResult, $"{provider} Results");
    }

    public void ReportFailed(DbProvider provider, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        Console.Error.WriteLine($"Error executing query on {provider}: {exception.Message}");
    }
    

    public void ReportCompleted()
    {
        Console.WriteLine();
    }

    private static void PrintCompiledQuery(DbProvider provider, CompiledQuery compiledQuery)
    {
        Console.WriteLine($"--- {provider} (compiled) ---");
        Console.WriteLine(compiledQuery.Sql);
        Console.WriteLine("Bindings:");

        for (var index = 0; index < compiledQuery.Bindings.Count; index++)
        {
            Console.WriteLine($"  [{index}] = {compiledQuery.Bindings[index]}");
        }

        Console.WriteLine();
    }
}
