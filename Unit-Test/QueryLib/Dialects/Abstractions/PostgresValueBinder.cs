using QueryLib.Demo;

namespace QueryLib.Dialects.Abstractions;

public sealed class PostgresValueBinder : IValueBinder
{
    public object? Bind(object? value) => value;
    public DbProvider DbType => DbProvider.PostgreSql;
}
