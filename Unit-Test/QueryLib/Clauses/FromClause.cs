using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Clauses;

/// <summary>Stores, validates, and renders the table in a FROM clause.</summary>
public sealed class FromClause : IQueryClause
{
    private string? _table;

    public int Order => 10;

    public string Table => _table
        ?? throw new InvalidOperationException(
            "From(...) must be called before compiling the query.");

    public void SetTable(string table)
    {
        if (string.IsNullOrWhiteSpace(table))
        {
            throw new ArgumentException("Table name cannot be empty.", nameof(table));
        }

        _table = table;
    }

    public RenderOutput Render(
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings) =>
        new($"FROM {quoter.Quote(Table)}", bindings);
}
