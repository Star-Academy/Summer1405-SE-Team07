namespace QueryLib.Demo;
public sealed class QueryResult
{
    public IReadOnlyCollection<string> ColumnNames { get; }
    public IReadOnlyCollection<Dictionary<string, object?>> Rows { get; }

    public QueryResult(
        IReadOnlyCollection<string> columnNames,
        IReadOnlyCollection<Dictionary<string, object?>> rows)
    {
        ColumnNames = columnNames;
        Rows = rows;
    }
}