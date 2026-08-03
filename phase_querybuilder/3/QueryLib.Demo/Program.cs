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
        new PostgresQueryRunner(),
        "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=mohaymen"
    ),

    new DatabaseConfiguration(
        "SQL Server",
        new SqlServerCompiler(),
        new SqlServerQueryRunner(),
        "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True"
    )
};

foreach (var db in databases)
{
    Console.WriteLine($"========== {db.Name} ==========");

    // Compile
    CompiledQuery compiledQuery = db.Compiler.Compile(query);

    PrintCompiled(db.Name, compiledQuery);

    // Execute
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

    for (int i = 0; i < result.Bindings.Count; i++)
    {
        Console.WriteLine($"  [{i}] = {result.Bindings[i]}");
    }

    Console.WriteLine();
}

public class DatabaseConfiguration
{
    public string Name { get; }

    public ICompiler Compiler { get; }

    public IQueryRunner Runner { get; }

    public string ConnectionString { get; }

    public DatabaseConfiguration(
        string name,
        ICompiler compiler,
        IQueryRunner runner,
        string connectionString)
    {
        Name = name;
        Compiler = compiler;
        Runner = runner;
        ConnectionString = connectionString;
    }
}





