using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.Postgres;

public sealed class PostgresDialect : ISqlDialect
{
    public IIdentifierQuoter IdentifierQuoter { get; }
    public IParameterPlaceholderFactory ParameterPlaceholderFactory { get; }
    public IValueBinder ValueBinder { get; }

    public PostgresDialect(
        IIdentifierQuoter identifierQuoter,
        IParameterPlaceholderFactory parameterPlaceholderFactory,
        IValueBinder valueBinder)
    {
        IdentifierQuoter = identifierQuoter ?? throw new ArgumentNullException(nameof(identifierQuoter));
        ParameterPlaceholderFactory = parameterPlaceholderFactory ?? throw new ArgumentNullException(nameof(parameterPlaceholderFactory));
        ValueBinder = valueBinder ?? throw new ArgumentNullException(nameof(valueBinder));
    }
}