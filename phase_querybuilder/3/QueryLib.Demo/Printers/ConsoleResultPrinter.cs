using System.Data.Common;

namespace QueryLib.Demo;

public class ConsoleResultPrinter : IResultPrinter
{
    public async Task PrintAsync(DbDataReader reader, string header)
    {
        Console.WriteLine($"--- {header} ---");

        while (await reader.ReadAsync())
        {
            var values = new List<string>();

            for (int col = 0; col < reader.FieldCount; col++)
            {
                values.Add($"{reader.GetName(col)}={reader.GetValue(col)}");
            }

            Console.WriteLine(string.Join(", ", values));
        }

        Console.WriteLine();
    }
}
