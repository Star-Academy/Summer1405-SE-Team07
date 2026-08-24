using QueryLib.Dialects.SqlServer;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerFromClauseRenderer : FromClauseRenderer
{
    public SqlServerFromClauseRenderer(SqlServerIdentifierQuoter quoter)
        : base(DbProvider.SqlServer, quoter)
    {
    }
}

