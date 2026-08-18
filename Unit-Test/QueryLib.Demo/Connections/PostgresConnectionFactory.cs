using System.Data.Common;
using Npgsql;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public class PostgresConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(string? connectionString)
    {
        _connectionString =  connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<DbConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        return connection;
    }
}
