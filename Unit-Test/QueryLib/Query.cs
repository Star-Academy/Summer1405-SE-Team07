using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;

namespace QueryLib;

public sealed class Query
{
    private readonly List<string> _columns = [];
    private readonly List<Condition> _conditions = [];

    private readonly SelectClause _selectClause;
    private FromClause? _fromClause;
    private WhereClause? _whereClause;

    public Query()
    {
        _selectClause = new SelectClause { Columns = _columns };
    }
    
    public IReadOnlyCollection<IQueryClause> Clauses
    {
        get
        {
            var clauses = new List<IQueryClause>();

            if (_fromClause is not null)
            {
                clauses.Add(_fromClause);
            }

            clauses.Add(_selectClause);

            if (_whereClause is not null)
            {
                clauses.Add(_whereClause);
            }

            return clauses;
        }
    }

    public Query From(string table)
    {
        _fromClause = new FromClause { Table = table };
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
        _whereClause ??= new WhereClause { Conditions = _conditions };

        return this;
    }
}