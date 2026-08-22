using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.SqlServer;

public sealed class SqlServerDialect : ISqlDialect
{
    public IIdentifierQuoter IdentifierQuoter { get; }
    public IParameterPlaceholderFactory ParameterPlaceholderFactory { get; }
    public IValueBinder ValueBinder { get; }

    public SqlServerDialect(
        IIdentifierQuoter identifierQuoter,
        IParameterPlaceholderFactory parameterPlaceholderFactory,
        IValueBinder valueBinder)
    {
        IdentifierQuoter = identifierQuoter ?? throw new ArgumentNullException(nameof(identifierQuoter));
        ParameterPlaceholderFactory = parameterPlaceholderFactory ?? throw new ArgumentNullException(nameof(parameterPlaceholderFactory));
        ValueBinder = valueBinder ?? throw new ArgumentNullException(nameof(valueBinder));
    }
}