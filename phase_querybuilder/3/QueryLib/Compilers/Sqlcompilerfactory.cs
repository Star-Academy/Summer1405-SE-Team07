

using QueryLib.Compilers.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Dialects.SqlServer;

namespace QueryLib.Compilers;

public class SqlCompilerFactory
{
    
    
    public static ICompiler Create(string compilertype)
    {
        switch (compilertype)
        {
            case "postgres":
                ISqlDialect postgresdialect = new PostgresDialect();
                return new SqlCompiler(
                    postgresdialect.IdentifierQuoter,
                    postgresdialect.ParameterPlaceholderFactory,
                    postgresdialect.ValueBinder);
            case "sqlserver":
                ISqlDialect sqlserverdialect = new SqlServerDialect();
                return new SqlCompiler(
                    sqlserverdialect.IdentifierQuoter,
                    sqlserverdialect.ParameterPlaceholderFactory,
                    sqlserverdialect.ValueBinder);
            default:
                throw new NotImplementedException();
        }
        
    }
}
