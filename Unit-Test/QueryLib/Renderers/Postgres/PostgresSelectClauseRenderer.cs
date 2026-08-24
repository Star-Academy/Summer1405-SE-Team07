using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresSelectClauseRenderer : IClauseRenderer
{
    private readonly PostgresIdentifierQuoter _quoter;

    public DbProvider Provider => DbProvider.PostgreSql;
    public ClauseKind ClauseKind => ClauseKind.Select;

    public PostgresSelectClauseRenderer(PostgresIdentifierQuoter quoter)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
    }

    public RenderOutput Render(IQueryClause clause)
    {
        var selectClause = (SelectClause)clause;

        var selection = selectClause.Columns.Count == 0
            ? "*"
            : string.Join(", ", selectClause.Columns.Select(_quoter.Quote));

        return new RenderOutput($"SELECT {selection}", Array.Empty<object?>());
    }
}

