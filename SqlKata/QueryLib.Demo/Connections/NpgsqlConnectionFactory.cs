using System.Data.Common;
using Npgsql;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    public DbProvider Provider => DbProvider.PostgreSql;

    public DbConnection Create(string connectionString)
        => new NpgsqlConnection(connectionString);
}
