using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses;

public sealed class FromClause : IQueryClause
{
    private string? _table;
    public int Order => 10;
    public ClauseKind Kind => ClauseKind.From;

    public string Table => _table
                           ?? throw new InvalidOperationException(
                               "From(...) must be called before compiling the query.");

    public void SetTable(string table)
    {
        if (string.IsNullOrWhiteSpace(table))
        {
            throw new ArgumentException(
                "Table name cannot be empty.",
                nameof(table));
        }

        _table = table;
    }
}