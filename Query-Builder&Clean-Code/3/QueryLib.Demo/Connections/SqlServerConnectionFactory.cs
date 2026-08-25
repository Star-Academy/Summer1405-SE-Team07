
using System.Data.Common;
using Microsoft.Data.SqlClient;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<DbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        return connection;
    }
}
