using System.Data.Common;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public sealed class ConnectionFactoryProvider : IDbConnectionFactoryProvider
{
    private readonly Func<string, DbConnection> _factory;

    public ConnectionFactoryProvider(Func<string, DbConnection> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public DbConnection CreateConnection(string connectionString) => _factory(connectionString);
}