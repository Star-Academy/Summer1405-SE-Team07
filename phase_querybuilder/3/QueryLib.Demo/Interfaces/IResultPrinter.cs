namespace QueryLib.Demo.Interfaces;

public interface IResultPrinter
{
    Task PrintAsync(QueryResult result, string header);
}
