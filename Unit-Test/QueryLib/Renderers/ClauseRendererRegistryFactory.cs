using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class ClauseRendererRegistryFactory : IClauseRendererRegistryFactory
{
    private readonly Dictionary<DbProvider, ClauseRendererRegistry> _registries;

    public ClauseRendererRegistryFactory(IEnumerable<IClauseRenderer> renderers)
    {
        _registries = renderers
            .GroupBy(renderer => renderer.Provider)
            .ToDictionary(
                group => group.Key,
                group => new ClauseRendererRegistry(group.Key, group));
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
