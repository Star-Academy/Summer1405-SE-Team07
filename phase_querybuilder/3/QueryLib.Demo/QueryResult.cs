public sealed class QueryResult
{
    private readonly List<string> _columnNames = [];
    private readonly List<Dictionary<string, object?>> _rows = [];

    public IReadOnlyCollection<string> ColumnNames => _columnNames;
    public IReadOnlyCollection<Dictionary<string, object?>> Rows => _rows;

    internal void AddColumn(string name)
    {
        _columnNames.Add(name);
    }

    internal void AddRow(Dictionary<string, object?> row)
    {
        _rows.Add(row);
    }
}