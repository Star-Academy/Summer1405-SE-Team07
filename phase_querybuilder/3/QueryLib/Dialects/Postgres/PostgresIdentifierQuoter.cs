namespace QueryLib.Dialects.Postgres;

public sealed class PostgresIdentifierQuoter : IIdentifierQuoter
{
    public string Quote(string identifier) => $"\"{identifier}\"";
}

