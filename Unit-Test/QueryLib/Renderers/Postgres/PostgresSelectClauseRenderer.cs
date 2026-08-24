using QueryLib.Dialects.Postgres;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresSelectClauseRenderer : SelectClauseRenderer
{
    public PostgresSelectClauseRenderer(PostgresIdentifierQuoter quoter)
        : base(DbProvider.PostgreSql, quoter)
    {
    }
}

