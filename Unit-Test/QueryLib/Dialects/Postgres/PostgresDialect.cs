using QueryLib.Dialects.Abstractions;

namespace QueryLib.Dialects.Postgres;

public sealed class PostgresDialect : ISqlDialect
{
    public IIdentifierQuoter IdentifierQuoter { get; } =
        new PostgresIdentifierQuoter();

    public IParameterPlaceholderFactory ParameterPlaceholderFactory { get; } =
        new PostgresParameterPlaceholderFactory();

    public IValueBinder ValueBinder { get; } =
        new PassthroughValueBinder();
}







