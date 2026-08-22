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
        services.AddKeyedSingleton<IValueBinder, PassthroughValueBinder>("postgres");

        services.AddKeyedSingleton<IIdentifierQuoter, SqlServerIdentifierQuoter>("sqlserver");
        services.AddKeyedSingleton<IParameterPlaceholderFactory, SqlServerParameterPlaceholderFactory>("sqlserver");
        services.AddKeyedSingleton<IValueBinder, SqlServerBooleanAsIntValueBinder>("sqlserver");

        services.AddKeyedSingleton<ISqlDialect, PostgresDialect>("postgres", (sp, key) =>
            new PostgresDialect(
                sp.GetRequiredKeyedService<IIdentifierQuoter>("postgres"),
                sp.GetRequiredKeyedService<IParameterPlaceholderFactory>("postgres"),
                sp.GetRequiredKeyedService<IValueBinder>("postgres")));

        services.AddKeyedSingleton<ISqlDialect, SqlServerDialect>("sqlserver", (sp, key) =>
            new SqlServerDialect(
                sp.GetRequiredKeyedService<IIdentifierQuoter>("sqlserver"),
                sp.GetRequiredKeyedService<IParameterPlaceholderFactory>("sqlserver"),
                sp.GetRequiredKeyedService<IValueBinder>("sqlserver")));
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
        services.AddKeyedSingleton<ICompiler>("postgres", (sp, key) =>
            new SqlCompiler(
                sp.GetRequiredKeyedService<IValueBinder>("postgres"),
                sp.GetRequiredKeyedService<ClauseRendererRegistry>("postgres")));

        services.AddKeyedSingleton<ICompiler>("sqlserver", (sp, key) =>
            new SqlCompiler(
                sp.GetRequiredKeyedService<IValueBinder>("sqlserver"),
                sp.GetRequiredKeyedService<ClauseRendererRegistry>("sqlserver")));

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