namespace QueryLib.Demo;
public sealed class QueryResult
{
    public required IReadOnlyCollection<string> ColumnNames { get; init; }
    public required IReadOnlyCollection<Dictionary<string, object?>> Rows { get; init; }
    
}