using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public abstract class SelectClauseRenderer : IClauseRenderer
{
    private readonly IIdentifierQuoter _quoter;

    public DbProvider Provider { get; }
    public ClauseKind ClauseKind => ClauseKind.Select;

    protected SelectClauseRenderer(DbProvider provider, IIdentifierQuoter quoter)
    {
        Provider = provider;
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public virtual RenderOutput Render(IQueryClause clause)
    {
        var selectClause = (SelectClause)clause;

        var selection = selectClause.Columns.Count == 0
            ? "*"
            : string.Join(", ", selectClause.Columns.Select(_quoter.Quote));

        return new RenderOutput($"SELECT {selection}", Array.Empty<object?>());
    }
}
