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

    public ClauseKind ClauseKind => ClauseKind.From;
    
    public RenderOutput Render(IQueryClause clause)
    {
        var fromClause = (FromClause)clause;
        return new RenderOutput($"FROM {_quoter.Quote(fromClause.Table)}", Array.Empty<object?>());
    }
}