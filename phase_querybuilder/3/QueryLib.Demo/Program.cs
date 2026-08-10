using QueryLib.Compilers;
using QueryLib.Demo.Connections;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Printers;
using QueryLib.Demo.QueryRunners;
using QueryLib.Compilers.Abstractions;

namespace QueryLib.Demo;

public static class Program
{
    
    
    public static async Task Main(string[] args)
    {
        // var wherecluasee = new WhereClause(); 
        // wherecluasee.Add(new Condition{Column = "FirstName" , Value = "John"});
        // ocp 
        
        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName", "LastName")
            .Where("IsMale", true);
        // .AddClause(wherecluasee); ocp example

        
        var dbConfigurations = new List<DbConfiguration>
        {
            new DbConfiguration(DbProvider.PostgreSQL , "Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen"),
            new DbConfiguration(DbProvider.Sqlserver , "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True"),
        };
        
        IResultPrinter printer = new ConsoleResultPrinter();

        ICompiler compiler;
        IQueryRunner runner;
        IDbConnectionFactory connectionFactory;
        
        var factory = new SqlCompilerFactory();
        
        
        foreach (var dbConfig in dbConfigurations)
        {
            switch (dbConfig.provider)
            {
                case (DbProvider.PostgreSQL):
                    compiler = factory.Create("postgres");
                    runner = new PostgresQueryRunner();
                    connectionFactory = new PostgresConnectionFactory(dbConfig.connectionString);
                    break;
                
                case(DbProvider.Sqlserver):
                    compiler = factory.Create("sqlserver");
                    runner = new SqlServerQueryRunner();
                    connectionFactory = new SqlServerConnectionFactory(dbConfig.connectionString);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(dbConfig.provider), dbConfig.provider, null);
            }
            
            Console.WriteLine($"========== {dbConfig.provider} ==========");
            
            var compiledQuery = compiler.Compile(query);
            PrintCompiled(dbConfig.provider.ToString(), compiledQuery);

            try
            {
                await using var connection = await connectionFactory.CreateConnectionAsync();
                await connection.OpenAsync();
                var reader = await runner.RunAsync(compiledQuery, connection);

                await printer.PrintAsync(reader, $"{dbConfig.provider.ToString()} Results");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error executing query on {dbConfig.provider.ToString()}: {exception.Message}");
            }

            Console.WriteLine();
        }
    }

    public static void PrintCompiled(string label, CompiledQuery result)
    {
        Console.WriteLine($"--- {label} (compiled) ---");
        Console.WriteLine(result.Sql);

        Console.WriteLine("Bindings:");

        var index = 0;
        foreach (var binding in result.Bindings)
        {
            Console.WriteLine($"  [{index++}] = {binding}");
        }

        Console.WriteLine();
    }
}
