

using QueryLib.Dialects.Postgres;
using QueryLib.Dialects.SqlServer;
using QueryLib.Dialects.Abstractions;



namespace QueryLib.Compilers;

public static class SqlCompilerFactory
{
    public static SqlCompiler CreatePostgres() =>
        new SqlCompiler(
            new PostgresIdentifierQuoter(),
            new PostgresParameterPlaceholderFactory(),
            new PassthroughValueBinder());

    public static SqlCompiler CreateSqlServer() =>
        new SqlCompiler(
            new SqlServerIdentifierQuoter(),
            new SqlServerParameterPlaceholderFactory(),
            new SqlServerBooleanAsIntValueBinder());
}
