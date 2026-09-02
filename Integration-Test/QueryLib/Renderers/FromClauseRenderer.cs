using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class FromClauseRenderer : IClauseRenderer
{
    private readonly IIdentifierQuoter _quoter;

    public DbProvider Provider { get; }
    public ClauseKind ClauseKind => ClauseKind.From;
    
    public FromClauseRenderer(DbProvider provider, IIdentifierQuoter quoter)
    {
        Provider = provider;
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public RenderOutput Render(IQueryClause clause)
    {
        var fromClause = (FromClause)clause;
        return new RenderOutput($"FROM {_quoter.Quote(fromClause.Table)}", Array.Empty<object?>());
    }
}