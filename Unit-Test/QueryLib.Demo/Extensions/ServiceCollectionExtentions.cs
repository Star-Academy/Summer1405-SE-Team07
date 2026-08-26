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
using QueryLib.Renderers.Abstractions;

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
        services.AddKeyedSingleton<IIdentifierQuoter, PostgresIdentifierQuoter>(DbProvider.PostgreSql);
        services.AddKeyedSingleton<IParameterPlaceholderFactory, PostgresParameterPlaceholderFactory>(DbProvider.PostgreSql);
        services.AddSingleton<IValueBinder, PostgresValueBinder>();

        services.AddKeyedSingleton<IIdentifierQuoter, SqlServerIdentifierQuoter>(DbProvider.SqlServer);
        services.AddKeyedSingleton<IParameterPlaceholderFactory, SqlServerParameterPlaceholderFactory>(DbProvider.SqlServer);
        services.AddSingleton<IValueBinder, SqlServerValueBinder>();

        services.AddSingleton<IValueBinderFactory, ValueBinderFactory>();
    }

    private static void AddRenderers(IServiceCollection services)
    {
        foreach (var provider in new[] { DbProvider.PostgreSql, DbProvider.SqlServer })
        {
            services.AddSingleton(sp => BuildRegistry(sp, provider));
        }

        services.AddSingleton<IClauseRendererRegistryFactory, ClauseRendererRegistryFactory>();
    }

    private static ClauseRendererRegistry BuildRegistry(IServiceProvider sp, DbProvider provider)
    {
        var quoter = sp.GetRequiredKeyedService<IIdentifierQuoter>(provider);
        var placeholders = sp.GetRequiredKeyedService<IParameterPlaceholderFactory>(provider);

        var renderers = new IClauseRenderer[]
        {
            new SelectClauseRenderer(provider, quoter),
            new FromClauseRenderer(provider, quoter),
            new WhereClauseRenderer(provider, quoter, placeholders),
        };

        return new ClauseRendererRegistry(provider, renderers);
    }

    private static void AddCompilers(IServiceCollection services)
    {
        services.AddSingleton<ICompiler, SqlCompiler>();
    }

    private static void AddQueryRunners(IServiceCollection services)
    {
        services.AddSingleton<IQueryRunner, PostgresQueryRunner>();
        services.AddSingleton<IQueryRunner, SqlServerQueryRunner>();
        services.AddSingleton<IQueryRunnerFactory, QueryRunnerFactory>();
    }

    private static void AddConnectionFactories(IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
        services.AddSingleton<IDbConnectionFactoryResolver, DbConnectionFactoryResolver>();
    }

    private static void AddExecutionServices(IServiceCollection services)
    {
        services.AddSingleton<IDatabaseQueryExecutor, DatabaseQueryExecutor>();
        services.AddSingleton<IQueryExecutionReporter, ConsoleQueryExecutionReporter>();
        services.AddSingleton<IResultPrinter, ConsoleResultPrinter>();
        services.AddSingleton<QueryExecutionService>();
    }
}