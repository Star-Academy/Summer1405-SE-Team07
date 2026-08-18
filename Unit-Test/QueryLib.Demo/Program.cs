using QueryLib.Compilers;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Printers;
using Microsoft.Extensions.DependencyInjection;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Execution.Abstractions;


namespace QueryLib.Demo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName", "LastName")
            .Where("IsMale", true);
        
        
        var dbConfigurations = new List<DbConfiguration>
        {
            new(DbProvider.PostgreSql, "Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen"),
            new(DbProvider.SqlServer, "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;Encrypt=False;TrustServerCertificate=True"),
        };
        
        
        
        var services = new ServiceCollection();
        services.AddSingleton<IDatabaseQueryExecutor, DatabaseQueryExecutor>();
        
        services.AddSingleton<
            IQueryExecutionDependencyFactory,
            QueryExecutionDependencyFactory>();
        
        services.AddSingleton<ISqlCompilerFactory, SqlCompilerFactory>();
        
        services.AddSingleton<
            IQueryExecutionReporter,
            ConsoleQueryExecutionReporter>();
        
        services.AddSingleton<IResultPrinter, ConsoleResultPrinter>();
        
        
        services.AddSingleton<QueryExecutionService>();
        
        using var serviceProvider = services.BuildServiceProvider();
        
        
        var executionService =
            serviceProvider.GetRequiredService<QueryExecutionService>();
        
        await executionService.ExecuteAsync(query, dbConfigurations);
    }
}
