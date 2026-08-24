using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class ClauseRendererRegistryFactory : IClauseRendererRegistryFactory
{
    private readonly Dictionary<DbProvider, ClauseRendererRegistry> _registries;

    public ClauseRendererRegistryFactory(IEnumerable<ClauseRendererRegistry> registries)
    {
        _registries = registries.ToDictionary(registry => registry.Provider);
    }

    public ClauseRendererRegistry GetRegistry(DbProvider provider) =>
        _registries.TryGetValue(provider, out var registry)
            ? registry
            : throw new KeyNotFoundException($"No renderer registry registered for provider {provider}.");
}