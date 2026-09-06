using Npgsql;
using QueryLib.Compilers;

namespace QueryLib.IntegrationTest.Infrastructure.Postgres;

public sealed class PostgresCompiledQueryExecutor : IAsyncDisposable
{
    private readonly NpgsqlConnection _connection;

    public PostgresCompiledQueryExecutor(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
    }

    public Task OpenAsync() => _connection.OpenAsync();

    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(CompiledQuery compiledQuery)
    {
        await using var command = new NpgsqlCommand(compiledQuery.Sql, _connection);
        foreach (var binding in compiledQuery.Bindings)
        {
            command.Parameters.Add(new NpgsqlParameter { Value = binding ?? DBNull.Value });
        }

        await using var reader = await command.ExecuteReaderAsync();
        return await CompiledQueryRowReader.ReadAllAsync(reader);
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
