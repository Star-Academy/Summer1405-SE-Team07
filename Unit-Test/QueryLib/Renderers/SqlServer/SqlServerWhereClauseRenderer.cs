using QueryLib.Dialects.SqlServer;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerWhereClauseRenderer : WhereClauseRenderer
{
    public SqlServerWhereClauseRenderer(
        SqlServerIdentifierQuoter quoter,
        SqlServerParameterPlaceholderFactory placeholders)
        : base(DbProvider.SqlServer, quoter, placeholders)
    {
    }
}

