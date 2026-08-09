namespace QueryLib.Demo.Abstractions;

public interface IResultPrinter
{
    Task PrintAsync(QueryResult result, string header);
}
