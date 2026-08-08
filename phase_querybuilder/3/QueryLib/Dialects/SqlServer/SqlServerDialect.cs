using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerDialect : ISqlDialect
{
    public IIdentifierQuoter IdentifierQuoter { get; } =
        new SqlServerIdentifierQuoter();

    public IParameterPlaceholderFactory ParameterPlaceholderFactory { get; } =
        new SqlServerParameterPlaceholderFactory();

    public IValueBinder ValueBinder { get; } =
        new SqlServerBooleanAsIntValueBinder();
}
