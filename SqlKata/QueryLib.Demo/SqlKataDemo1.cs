using SqlKata.Compilers;

namespace QueryLib.Demo;

public static class SqlKataDemo1
{
    public static void Run()
    {
        var query = new SqlKata.Query("Student")
            .Select("StudentNumber", "FirstName", "LastName")
            .Where("IsMale", true)
            .Where("Age", 20);

        var postgresCompiler = new PostgresCompiler();
        var postgresResult = postgresCompiler.Compile(query);

        Console.WriteLine("===== PostgreSQL =====");
        Console.WriteLine(postgresResult.Sql);

        Console.WriteLine("Bindings:");

        foreach (var binding in postgresResult.Bindings)
        {
            Console.WriteLine(binding);
        }

        var sqlServerCompiler = new SqlServerCompiler();
        var sqlServerResult = sqlServerCompiler.Compile(query);

        Console.WriteLine();
        Console.WriteLine("===== SQL Server =====");
        Console.WriteLine(sqlServerResult.Sql);

        Console.WriteLine("Bindings:");

        foreach (var binding in sqlServerResult.Bindings)
        {
            Console.WriteLine(binding);
        }
    }
}