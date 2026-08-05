using Microsoft.Data.SqlClient;
using QueryLib.Compilers;

namespace QueryLib.Demo;

public sealed class SqlServerQueryRunner : IQueryRunner
{
    private readonly IResultPrinter _printer;

    public SqlServerQueryRunner(IResultPrinter printer)
    {
        _printer = printer;
    }

    public async Task RunAsync(CompiledQuery query, string connectionString)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(query.Sql, connection);

        for (int param = 0; param < query.Bindings.Count; param++)
        {
            command.Parameters.AddWithValue("@p" + param, query.Bindings[param] ?? DBNull.Value);
        }

        await using var reader = await command.ExecuteReaderAsync();
        await _printer.PrintAsync(reader, "SQL Server results");
    }
}
