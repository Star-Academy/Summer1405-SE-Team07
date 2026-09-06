using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.Postgres;

public sealed class PostgresValueBinder : IValueBinder
{
    public object? Bind(object? value) => value;
    public DbProvider DbType => DbProvider.PostgreSql;
}
