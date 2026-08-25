using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresFromClauseRenderer : FromClauseRenderer
{
    public PostgresFromClauseRenderer(PostgresIdentifierQuoter quoter)
        : base(DbProvider.PostgreSql, quoter)
    {
    }
}
