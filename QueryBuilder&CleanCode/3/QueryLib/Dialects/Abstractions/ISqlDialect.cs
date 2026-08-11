namespace QueryLib.Dialects.Abstractions;

public interface ISqlDialect
{
    IIdentifierQuoter IdentifierQuoter { get; }

    IParameterPlaceholderFactory ParameterPlaceholderFactory { get; }

    IValueBinder ValueBinder { get; }
}
