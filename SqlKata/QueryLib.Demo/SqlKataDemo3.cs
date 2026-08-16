using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

public static class SqlKataDemo3
{ 
    public static async Task RunAsync()
    {
        var query = new SqlKata.Query("Student")
            .Select(
                "StudentNumber",
                "FirstName",
                "LastName")
            .Where("IsMale", true)
            .WhereIn("StudentNumber", new[]
            {
                "98100203",
                "98100207",
                "98100205"
            })
            .OrderBy("LastName")
            .Limit(10);

        const string connectionString =
            "Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen";

        await using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        var compiler = new PostgresCompiler();
        
        var result = compiler.Compile(query);
        
        Console.WriteLine("===== SQL =====");
        Console.WriteLine(result.Sql);

        Console.WriteLine();
        Console.WriteLine("===== Bindings =====");

        foreach (var binding in result.Bindings)
        {
            Console.WriteLine(binding);
        }
        
        Console.WriteLine();

        var db = new QueryFactory(connection, compiler, 30);

        var students = await db.GetAsync(query);

        foreach (var student in students)
        {
            Console.WriteLine(student);
        }
        
        Console.WriteLine();

    }
}