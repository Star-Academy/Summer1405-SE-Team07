using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Connections;

public sealed class DbConnectionFactoryResolver : IDbConnectionFactoryResolver
{
    private readonly Dictionary<DbProvider, IDbConnectionFactory> _factories;

    public DbConnectionFactoryResolver(IEnumerable<IDbConnectionFactory> factories)
    {
        _factories = factories.ToDictionary(f => f.Provider);
    }

    public IDbConnectionFactory GetFactory(DbProvider provider)
    {
        if (_factories.TryGetValue(provider, out var factory))
        {
            return factory;
        }
        throw new KeyNotFoundException($"No connection factory registered for provider {provider}.");
    }
}

