using System.Data.Common;
using Microsoft.Data.SqlClient;
using Npgsql;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public sealed class ConnectionFactoryProvider : IDbConnectionFactoryProvider
{
    private readonly Func<string, DbConnection> _factory;

    public ConnectionFactoryProvider(Func<string, DbConnection> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public DbConnection CreateConnection(DbConfiguration configuration)
    {
        switch (configuration.Provider)
        {
            case (DbProvider.PostgreSql):
                return new NpgsqlConnection();
            
            case (DbProvider.SqlServer):
                return new SqlConnection();
            
            default:
                throw new ArgumentOutOfRangeException();
            
        }
    }
}