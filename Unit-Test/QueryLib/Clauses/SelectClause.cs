using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses;

public sealed class SelectClause : IQueryClause
{
    private readonly List<string> _columns = new();

    public int Order => 0;

    public IReadOnlyCollection<string> Columns => _columns;

    public void Add(IEnumerable<string>? columns)
    {
        if (columns is not null)
        {
            _columns.AddRange(columns);
        }
    }
    
}