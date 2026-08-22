using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class FromClauseRenderer : IClauseRenderer
{
    private readonly IIdentifierQuoter _quoter;

    public FromClauseRenderer(IIdentifierQuoter quoter)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public Type ClauseType => typeof(FromClause);

    public RenderOutput Render(IQueryClause clause, IReadOnlyCollection<object?> bindings)
    {
        var fromClause = (FromClause)clause;
        return new RenderOutput($"FROM {_quoter.Quote(fromClause.Table)}", bindings);
    }
}