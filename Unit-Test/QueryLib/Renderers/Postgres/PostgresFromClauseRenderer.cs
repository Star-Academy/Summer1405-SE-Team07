using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresFromClauseRenderer : IClauseRenderer
{
    private readonly PostgresIdentifierQuoter _quoter;

    public DbProvider Provider => DbProvider.PostgreSql;
    public ClauseKind ClauseKind => ClauseKind.From;

    public PostgresFromClauseRenderer(PostgresIdentifierQuoter quoter)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public RenderOutput Render(IQueryClause clause)
    {
        var fromClause = (FromClause)clause;
        return new RenderOutput($"FROM {_quoter.Quote(fromClause.Table)}", Array.Empty<object?>());
    }
}

