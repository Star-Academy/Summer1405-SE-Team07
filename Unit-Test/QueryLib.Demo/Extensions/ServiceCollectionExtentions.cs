using System.Data.Common;
using Npgsql;
using Microsoft.Data.SqlClient;
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
        services.AddKeyedSingleton<IIdentifierQuoter, PostgresIdentifierQuoter>("postgres");
        services.AddKeyedSingleton<IParameterPlaceholderFactory, PostgresParameterPlaceholderFactory>("postgres");
        services.AddSingleton<IValueBinder, PostgresValueBinder>();
        services.AddSingleton<IValueBinderFactory, ValueBinderFactory>();
        services.AddKeyedSingleton<IIdentifierQuoter, SqlServerIdentifierQuoter>("sqlserver");
        services.AddKeyedSingleton<IParameterPlaceholderFactory, SqlServerParameterPlaceholderFactory>("sqlserver");
        services.AddSingleton<IValueBinder, SqlServerValueBinder>();
    }

    private static void AddRenderers(IServiceCollection services)
    {
        RegisterRenderersFor(services, "postgres");
        RegisterRenderersFor(services, "sqlserver");
    }

    private static void RegisterRenderersFor(IServiceCollection services, string dialectKey)
    {
        services.AddKeyedSingleton<IClauseRenderer>(dialectKey, (sp, key) =>
            new SelectClauseRenderer(sp.GetRequiredKeyedService<IIdentifierQuoter>(dialectKey)));

        services.AddKeyedSingleton<IClauseRenderer>(dialectKey, (sp, key) =>
            new FromClauseRenderer(sp.GetRequiredKeyedService<IIdentifierQuoter>(dialectKey)));

        services.AddKeyedSingleton<IClauseRenderer>(dialectKey, (sp, key) =>
            new WhereClauseRenderer(
                sp.GetRequiredKeyedService<IIdentifierQuoter>(dialectKey),
                sp.GetRequiredKeyedService<IParameterPlaceholderFactory>(dialectKey)));

        services.AddKeyedSingleton<ClauseRendererRegistry>(dialectKey, (sp, key) =>
            new ClauseRendererRegistry(sp.GetKeyedServices<IClauseRenderer>(dialectKey)));
    }

    private static void AddCompilers(IServiceCollection services)
    {
        services.AddSingleton<ICompiler, SqlCompiler>();
    }

    private static void AddQueryRunners(IServiceCollection services)
    {
        services.AddKeyedSingleton<IQueryRunner, PostgresQueryRunner>("postgres");
        services.AddKeyedSingleton<IQueryRunner, SqlServerQueryRunner>("sqlserver");
    }

    private static void AddConnectionFactories(IServiceCollection services)
    {
        services.AddKeyedSingleton<IDbConnectionFactoryProvider>("postgres", (sp, key) =>
            new ConnectionFactoryProvider(
                sp.GetRequiredKeyedService<Func<string, DbConnection>>("postgres")));

        services.AddKeyedSingleton<IDbConnectionFactoryProvider>("sqlserver", (sp, key) =>
            new ConnectionFactoryProvider(
                sp.GetRequiredKeyedService<Func<string, DbConnection>>("sqlserver")));
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