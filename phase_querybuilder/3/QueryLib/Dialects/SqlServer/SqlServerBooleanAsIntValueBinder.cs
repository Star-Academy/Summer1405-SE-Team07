namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerBooleanAsIntValueBinder : IValueBinder
{
    public object? Bind(object? value) => value is bool b ? (b ? 1 : 0) : value;
}

