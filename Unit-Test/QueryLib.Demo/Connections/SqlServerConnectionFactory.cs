using System.Data.Common;
using Microsoft.Data.SqlClient;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public sealed class SqlServerConnectionFactory : IDbConnectionFactory
{
    public DbConnection Create(string connectionString)
        => new SqlConnection(connectionString);
}

