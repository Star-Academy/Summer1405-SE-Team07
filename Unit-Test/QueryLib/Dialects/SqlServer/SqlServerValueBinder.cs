using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerValueBinder : IValueBinder
{
    public object? Bind(object? value) => value is bool b ? (b ? 1 : 0) : value;
    public DbProvider DbType => DbProvider.SqlServer;
}
