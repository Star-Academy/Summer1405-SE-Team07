using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerIdentifierQuoter : IIdentifierQuoter
{
    public string Quote(string identifier) => $"[{identifier}]";
}
