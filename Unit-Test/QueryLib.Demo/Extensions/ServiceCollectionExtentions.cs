using Microsoft.Extensions.DependencyInjection;
using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Connections;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Execution.Abstractions;
using QueryLib.Demo.Printers;
using QueryLib.Demo.QueryRunners;
using QueryLib.Dialects.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Dialects.SqlServer;
using QueryLib.Renderers;

namespace QueryLib.Demo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueryLibServices(this IServiceCollection services)
    {
        AddDialects(services);
        AddRenderers(services);
        AddCompilers(services);
        AddQueryRunners(services);
        AddConnectionFactories(services);
        AddExecutionServices(services);

        return services;
    }

    private static void AddDialects(IServiceCollection services)
    {
        services.AddKeyedSingleton<ISqlDialect, PostgresDialect>("postgres");
        services.AddKeyedSingleton<ISqlDialect, SqlServerDialect>("sqlserver");
    }

    private static void AddRenderers(IServiceCollection services)
    {
        services.AddSingleton<SelectClauseRenderer>();
        services.AddSingleton<FromClauseRenderer>();
        services.AddSingleton<WhereClauseRenderer>();
    }

    private static void AddCompilers(IServiceCollection services)
    {
        services.AddKeyedSingleton<ICompiler>("postgres", (sp, key) =>
        {
            var dialect = sp.GetRequiredKeyedService<ISqlDialect>("postgres");
            return new SqlCompiler(
                dialect.IdentifierQuoter,
                dialect.ParameterPlaceholderFactory,
                dialect.ValueBinder,
                sp.GetRequiredService<SelectClauseRenderer>(),
                sp.GetRequiredService<WhereClauseRenderer>(),
                sp.GetRequiredService<FromClauseRenderer>());
        });

        services.AddKeyedSingleton<ICompiler>("sqlserver", (sp, key) =>
        {
            var dialect = sp.GetRequiredKeyedService<ISqlDialect>("sqlserver");
            return new SqlCompiler(
                dialect.IdentifierQuoter,
                dialect.ParameterPlaceholderFactory,
                dialect.ValueBinder,
                sp.GetRequiredService<SelectClauseRenderer>(),
                sp.GetRequiredService<WhereClauseRenderer>(),
                sp.GetRequiredService<FromClauseRenderer>());
        });

        services.AddSingleton<ISqlCompilerFactory, SqlCompilerFactory>();
    }

    private static void AddQueryRunners(IServiceCollection services)
    {
        services.AddKeyedSingleton<IQueryRunner, PostgresQueryRunner>("postgres");
        services.AddKeyedSingleton<IQueryRunner, SqlServerQueryRunner>("sqlserver");
    }

    private static void AddConnectionFactories(IServiceCollection services)
    {
        services.AddKeyedSingleton<Func<string, IDbConnectionFactory>>("postgres",
            (sp, key) => connectionString => new PostgresConnectionFactory(connectionString));

        services.AddKeyedSingleton<Func<string, IDbConnectionFactory>>("sqlserver",
            (sp, key) => connectionString => new SqlServerConnectionFactory(connectionString));
    }

    private static void AddExecutionServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseQueryExecutor, DatabaseQueryExecutor>();
        services.AddSingleton<IQueryExecutionDependencyFactory, QueryExecutionDependencyFactory>();
        services.AddSingleton<IQueryExecutionReporter, ConsoleQueryExecutionReporter>();
        services.AddSingleton<IResultPrinter, ConsoleResultPrinter>();
        services.AddSingleton<QueryExecutionService>();
    }
}