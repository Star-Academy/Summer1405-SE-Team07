using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Renderers.Abstractions;

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
