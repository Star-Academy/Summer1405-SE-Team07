using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryLib;


public sealed class Query
{
    private readonly SelectClause _selectClause = new();
    private readonly FromClause _fromClause = new();
    private WhereClause? _whereClause;
    private readonly List<IQueryClause> _clauses = new();


    public Query()
    {
        _clauses.Add(_selectClause);
        _clauses.Add(_fromClause);
    }

    public string Table => _fromClause.Table;
    public IReadOnlyCollection<string> Columns => _selectClause.Columns;
    public IReadOnlyCollection<IQueryClause> Clauses => _clauses;

    public Query From(string table)
    {
        _fromClause.SetTable(table);
        return this;
    }

    public Query Select(params string[]? columns)
    {
        _selectClause.Add(columns);
        return this;
    }

    public Query Where(string column, object? value)
    {
        _whereClause ??= AddNew(new WhereClause());
        _whereClause.Add(new Condition { Column = column, Value = value });
        return this;
    }

    public Query AddClause(IQueryClause clause)
    {
        ArgumentNullException.ThrowIfNull(clause);
        _clauses.Add(clause);
        return this;
    }

    private T AddNew<T>(T clause) where T : IQueryClause
    {
        _clauses.Add(clause);
        return clause;
    }
}
