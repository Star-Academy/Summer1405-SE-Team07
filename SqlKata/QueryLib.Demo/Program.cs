using QueryLib.Compilers;
using QueryLib.Demo.Execution;
using QueryLib.Demo.Printers;

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

        var dependencyFactory = new QueryExecutionDependencyFactory(new SqlCompilerFactory());
        var queryExecutor = new DatabaseQueryExecutor(dependencyFactory);
        var reporter = new ConsoleQueryExecutionReporter(new ConsoleResultPrinter());
        var executionService = new QueryExecutionService(queryExecutor, reporter);

        await executionService.ExecuteAsync(query, dbConfigurations);
        
        //SqlKataDemo1.Run();

        //SqlKataDemo2.Run();
        
        await SqlKataDemo3.RunAsync();

    }
}

