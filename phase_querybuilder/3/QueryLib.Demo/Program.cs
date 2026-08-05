using QueryLib;
using QueryLib.Compilers;
using QueryLib.Demo;

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName", "LastName")
    .Where("IsMale", true);

var databases = new List<DatabaseConfiguration>
{
    new DatabaseConfiguration(
        "PostgreSQL",
        new PostgresCompiler(),
        new PostgresQueryRunner(new ConsoleResultPrinter()),
        "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mohaymen"
    ),

    new DatabaseConfiguration(
        "SQL Server",
        new SqlServerCompiler(),
        new SqlServerQueryRunner(new ConsoleResultPrinter()),
        "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True"
    )
};

foreach (var db in databases)
{
    Console.WriteLine($"========== {db.Name} ==========");

    CompiledQuery compiledQuery = db.Compiler.Compile(query);

    PrintCompiled(db.Name, compiledQuery);

    if (!string.IsNullOrWhiteSpace(db.ConnectionString))
    {
        await db.Runner.RunAsync(compiledQuery, db.ConnectionString);
    }
    else
    {
        Console.WriteLine($"Skipping {db.Name}: Connection string not set.");
    }

    Console.WriteLine();
}

static void PrintCompiled(string label, CompiledQuery result)
{
    Console.WriteLine($"--- {label} (compiled) ---");
    Console.WriteLine(result.Sql);

    Console.WriteLine("Bindings:");

    foreach (var Binding in result.Bindings)
    {
        var index = 0;
        Console.WriteLine($"  [{index++}] = {Binding}");
    }

    Console.WriteLine();
}

