using QueryLib;
using QueryLib.Compilers;

var query = new Query()
    .From("Students")
    .Select("Id", "Name")
    .Where("IsMale", true)
    .Where("Age", 20);

CompiledQuery postgresResult = new PostgresCompiler().Compile(query);
CompiledQuery sqlServerResult = new SqlServerCompiler().Compile(query);

PrintResult("PostgreSQL", postgresResult);
PrintResult("SQL Server", sqlServerResult);

static void PrintResult(string label, CompiledQuery result)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine(result.Sql);
    Console.WriteLine("Bindings:");

    for (int i = 0; i < result.Bindings.Count; i++)
        Console.WriteLine($"  [{i}] = {result.Bindings[i]}");

    Console.WriteLine();
}
