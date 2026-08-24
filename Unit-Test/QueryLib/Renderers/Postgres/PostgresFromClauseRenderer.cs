using QueryLib.Dialects.Postgres;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresFromClauseRenderer : FromClauseRenderer
{
    public PostgresFromClauseRenderer(PostgresIdentifierQuoter quoter)
        : base(DbProvider.PostgreSql, quoter)
    {
    }
}

