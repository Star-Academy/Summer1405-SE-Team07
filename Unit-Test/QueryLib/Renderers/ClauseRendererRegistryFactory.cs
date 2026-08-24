using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class ClauseRendererRegistryFactory : IClauseRendererRegistryFactory
{
    private readonly Dictionary<DbProvider, ClauseRendererRegistry> _registries;

    public ClauseRendererRegistryFactory(IEnumerable<ClauseRendererRegistry> registries)
    {
        _registries = registries.ToDictionary(r => r.Provider);
    }

    public ClauseRendererRegistry GetRegistry(DbProvider provider)
    {
        if (_registries.TryGetValue(provider, out var registry))
        {
            return registry;
        }
        throw new KeyNotFoundException($"No renderer registry registered for provider {provider}.");
    }
}

