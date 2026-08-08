

using QueryLib.Compilers.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Compilers;

public static class SqlCompilerFactory
{
    public static ICompiler Create(ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        return new SqlCompiler(
            dialect.IdentifierQuoter,
            dialect.ParameterPlaceholderFactory,
            dialect.ValueBinder);
    }
}
