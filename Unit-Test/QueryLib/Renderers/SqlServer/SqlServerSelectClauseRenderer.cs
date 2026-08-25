using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.SqlServer;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerSelectClauseRenderer : SelectClauseRenderer
{
    public SqlServerSelectClauseRenderer(SqlServerIdentifierQuoter quoter)
        : base(DbProvider.SqlServer, quoter)
    {
    }
}
