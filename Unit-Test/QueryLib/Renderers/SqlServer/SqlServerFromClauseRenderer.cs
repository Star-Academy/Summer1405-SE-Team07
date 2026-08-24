using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.SqlServer;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerFromClauseRenderer : IClauseRenderer
{
    private readonly SqlServerIdentifierQuoter _quoter;

    public DbProvider Provider => DbProvider.SqlServer;
    public ClauseKind ClauseKind => ClauseKind.From;

    public SqlServerFromClauseRenderer(SqlServerIdentifierQuoter quoter)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public RenderOutput Render(IQueryClause clause)
    {
        var fromClause = (FromClause)clause;
        return new RenderOutput($"FROM {_quoter.Quote(fromClause.Table)}", Array.Empty<object?>());
    }
}

