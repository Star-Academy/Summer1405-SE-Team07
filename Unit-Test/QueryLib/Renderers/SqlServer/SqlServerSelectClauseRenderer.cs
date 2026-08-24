using QueryLib.Dialects.SqlServer;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerSelectClauseRenderer : SelectClauseRenderer
{
    public SqlServerSelectClauseRenderer(SqlServerIdentifierQuoter quoter)
        : base(DbProvider.SqlServer, quoter)
    {
    }
}

