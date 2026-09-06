using Microsoft.Data.SqlClient;
using QueryLib.Compilers;

namespace QueryLib.IntegrationTest.Infrastructure.SqlServer;

public sealed class SqlServerCompiledQueryExecutor : IAsyncDisposable
{
    private readonly SqlConnection _connection;

    public SqlServerCompiledQueryExecutor(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
    }

    public Task OpenAsync() => _connection.OpenAsync();

    public async Task<List<Dictionary<string, object?>>> ExecuteAsync(CompiledQuery compiledQuery)
    {
        await using var command = new SqlCommand(compiledQuery.Sql, _connection);
        for (var param = 0; param < compiledQuery.Bindings.Count; param++)
        {
            command.Parameters.Add(new SqlParameter($"@p{param}", compiledQuery.Bindings[param] ?? DBNull.Value));
        }

        await using var reader = await command.ExecuteReaderAsync();
        return await CompiledQueryRowReader.ReadAllAsync(reader);
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
