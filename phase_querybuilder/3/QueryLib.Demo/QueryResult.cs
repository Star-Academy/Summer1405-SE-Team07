namespace QueryLib.Demo;

public class QueryResult
{
    public List<string> ColumnNames { get; } = new();
    public List<Dictionary<string, object?>> Rows { get; } = new();
}