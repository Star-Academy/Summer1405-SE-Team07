using QueryLib.Dialects.Postgres;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresWhereClauseRenderer : WhereClauseRenderer
{
    public PostgresWhereClauseRenderer(
        PostgresIdentifierQuoter quoter,
        PostgresParameterPlaceholderFactory placeholders)
        : base(DbProvider.PostgreSql, quoter, placeholders)
    {
    }
}

