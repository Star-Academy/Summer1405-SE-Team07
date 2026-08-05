using QueryLib;
using QueryLib.Compilers;
using QueryLib.Demo;

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName", "LastName")
    .Where("IsMale", true);

IResultPrinter printer = new ConsoleResultPrinter();

var databases = new List<DatabaseEngine>
{
    new DatabaseEngine(
        "PostgreSQL",
        new PostgresCompiler(),
        new PostgresQueryRunner(),
        new PostgresConnectionFactory("Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mohaymen")
    ),

    new DatabaseEngine(
        "SQL Server",
        new SqlServerCompiler(),
        new SqlServerQueryRunner(),
        new SqlServerConnectionFactory("Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True")
    )
};

foreach (var db in databases)
{
    Console.WriteLine($"========== {db.Name} ==========");

    CompiledQuery compiledQuery = db.Compiler.Compile(query);
    PrintCompiled(db.Name, compiledQuery);

    try
    {
        await using var connection = await db.ConnectionFactory.CreateConnectionAsync();
        
        var reader = await db.Runner.RunAsync(compiledQuery, connection);

        await printer.PrintAsync(reader, $"{db.Name} Results");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error executing query on {db.Name}: {ex.Message}");
    }

    Console.WriteLine();
}

static void PrintCompiled(string label, CompiledQuery result)
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