using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.Postgres;

public sealed class PostgresIdentifierQuoter : IIdentifierQuoter
{
    public string Quote(string identifier) => $"\"{identifier}\"";
}
