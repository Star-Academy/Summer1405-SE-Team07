using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

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

    public RenderOutput Render(
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings)
    {
        var selection = _columns.Count == 0
            ? "*"
            : string.Join(", ", _columns.Select(quoter.Quote));

        return new RenderOutput($"SELECT {selection}", bindings);
    }
}
