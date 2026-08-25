using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresSelectClauseRenderer : SelectClauseRenderer
{
    public PostgresSelectClauseRenderer(PostgresIdentifierQuoter quoter)
        : base(DbProvider.PostgreSql, quoter)
    {
    }
}
