using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerIdentifierQuoter : IIdentifierQuoter
{
    public DbProvider Provider => DbProvider.SqlServer;
    public string Quote(string identifier) => $"[{identifier}]";
}
