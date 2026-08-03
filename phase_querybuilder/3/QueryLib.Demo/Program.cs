using QueryLib;
using QueryLib.Compilers;
using QueryLib.Demo;

var query = new Query()
    .From("Student")
    .Select("StudentNumber", "FirstName", "LastName")
    .Where("IsMale", true);

// DIP: the demo uses the ICompiler abstraction; concrete compilers are chosen only here.
ICompiler postgresCompiler = new PostgresCompiler();
ICompiler sqlServerCompiler = new SqlServerCompiler();

CompiledQuery postgresResult = postgresCompiler.Compile(query);
CompiledQuery sqlServerResult = sqlServerCompiler.Compile(query);

PrintCompiled("PostgreSQL", postgresResult);
PrintCompiled("SQL Server", sqlServerResult);

string postgresConnectionString="Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen";
string sqlServerConnectionString="Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True";

if (postgresConnectionString is not null)
{
    await QueryRunner.RunOnPostgresAsync(postgresResult, postgresConnectionString);
}
else
{
    Console.WriteLine("Skipping PostgreSQL execution: POSTGRES_CONNECTION_STRING not set.");
}

if (sqlServerConnectionString is not null)
{
    await QueryRunner.RunOnSqlServerAsync(sqlServerResult, sqlServerConnectionString);
}
else
{
    Console.WriteLine("Skipping SQL Server execution: SQLSERVER_CONNECTION_STRING not set.");
}

static void PrintCompiled(string label, CompiledQuery result)
{
    Console.WriteLine($"--- {label} (compiled) ---");
    Console.WriteLine(result.Sql);
    Console.WriteLine("Bindings:");

    for (int i = 0; i < result.Bindings.Count; i++)
        Console.WriteLine($"  [{i}] = {result.Bindings[i]}");

    Console.WriteLine();
}
