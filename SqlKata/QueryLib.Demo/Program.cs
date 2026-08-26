using Microsoft.Extensions.DependencyInjection;
using QueryLib.Demo.Extensions;
using QueryLib.Demo.Execution;

namespace QueryLib.Demo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // var query = new Query()
        //     .From("Student")
        //     .Select("StudentNumber", "FirstName", "LastName")
        //     .Where("IsMale", true);
        //
        // var dbConfigurations = new List<DbConfiguration>
        // {
        //     new(DbProvider.PostgreSql,
        //         "Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen"),
        //     new(DbProvider.SqlServer,
        //         "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;Encrypt=False;TrustServerCertificate=True"),
        // };
        //
        // var services = new ServiceCollection();
        // services.AddQueryLibServices();
        //
        // await using var serviceProvider = services.BuildServiceProvider();
        //
        // var executionService = serviceProvider.GetRequiredService<QueryExecutionService>();
        //
        // await executionService.ExecuteAsync(query, dbConfigurations);

        //SqlKataDemo1.Run();

        SqlKataDemo2.Run();

        await SqlKataDemo3.RunAsync();
    }
}
