using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryLib;

public sealed class Query
{
    private readonly List<string> _columns = [];
    private readonly List<Condition> _conditions = [];

    private readonly SelectClause _selectClause;
    private readonly FromClause _fromClause;
    private WhereClause? _whereClause;

    private readonly List<IQueryClause> _clauses = [];

    public Query()
    {
        _selectClause = new SelectClause { Columns = _columns };

        _fromClause = new FromClause();

        _clauses.Add(_selectClause);
        _clauses.Add(_fromClause);
    }

    public string Table => _fromClause.Table;

    public IReadOnlyCollection<string> Columns => _columns;

    public IReadOnlyCollection<IQueryClause> Clauses => _clauses;

    public Query From(string table)
    {
        _fromClause.SetTable(table);
        return this;
    }

    public Query Select(params string[]? columns)
    {
        if (columns is not null)
        {
            _columns.AddRange(columns);
        }

        return this;
    }

    public Query Where(string column, object? value)
    {
        _conditions.Add(new Condition { Column = column, Value = value });
        _whereClause ??= AddNew(new WhereClause { Conditions = _conditions });

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