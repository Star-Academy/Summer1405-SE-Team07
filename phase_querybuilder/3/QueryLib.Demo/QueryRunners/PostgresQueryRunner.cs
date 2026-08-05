using Npgsql;
using QueryLib.Compilers;

namespace QueryLib.Demo
{
public sealed class PostgresQueryRunner : IQueryRunner
{
    private readonly IResultPrinter _printer;
    public PostgresQueryRunner(IResultPrinter printer)
    {
        _printer = printer;
    }
    public async Task RunAsync(CompiledQuery query, string connectionString)
    {
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        await using var command = dataSource.CreateCommand(query.Sql);

        foreach (var value in query.Bindings)
        {
            command.Parameters.AddWithValue(value ?? DBNull.Value);
        }

        await using var reader = await command.ExecuteReaderAsync();
        await _printer.PrintAsync(reader, "PostgreSQL results");
    }
}
}