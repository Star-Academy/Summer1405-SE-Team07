using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.SqlServer;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerSelectClauseRenderer : IClauseRenderer
{
    private readonly SqlServerIdentifierQuoter _quoter;

    public DbProvider Provider => DbProvider.SqlServer;
    public ClauseKind ClauseKind => ClauseKind.Select;

    public SqlServerSelectClauseRenderer(SqlServerIdentifierQuoter quoter)
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

