using QueryLib.Compilers.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Dialects.SqlServer;
using QueryLib.Renderers;

namespace QueryLib.Compilers;

public class SqlCompilerFactory : ISqlCompilerFactory
{
    public ICompiler Create(string compilertype)
    {
        var  selectClauseRenderer = new SelectClauseRenderer();
        var  fromClauseRenderer = new FromClauseRenderer();
        var  whereClauseRenderer = new WhereClauseRenderer();
        switch (compilertype)
        {
            case "postgres":
                ISqlDialect postgresdialect = new PostgresDialect();
                return new SqlCompiler(
                    postgresdialect.IdentifierQuoter,
                    postgresdialect.ParameterPlaceholderFactory,
                    postgresdialect.ValueBinder,
                    selectClauseRenderer,
                    whereClauseRenderer,
                    fromClauseRenderer);
            case "sqlserver":
                ISqlDialect sqlserverdialect = new SqlServerDialect();
                return new SqlCompiler(
                    sqlserverdialect.IdentifierQuoter,
                    sqlserverdialect.ParameterPlaceholderFactory,
                    sqlserverdialect.ValueBinder,
                    selectClauseRenderer,
                    whereClauseRenderer,
                    fromClauseRenderer);
            default:
                throw new NotImplementedException();
        }
        
    }
}
