using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class SelectClauseRenderer : IClauseRenderer
{
    private readonly IIdentifierQuoter _quoter;

    public SelectClauseRenderer(IIdentifierQuoter quoter)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public Type ClauseType => typeof(SelectClause);

    public RenderOutput Render(IQueryClause clause, IReadOnlyCollection<object?> bindings)
    {
        var selectClause = (SelectClause)clause;

        var selection = selectClause.Columns.Count == 0
            ? "*"
            : string.Join(", ", selectClause.Columns.Select(_quoter.Quote));

        return new RenderOutput($"SELECT {selection}", bindings);
    }
}