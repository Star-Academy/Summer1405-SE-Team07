using QueryLib.Clauses.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class ClauseRendererRegistry
{
    private readonly Dictionary<ClauseKind, IClauseRenderer> _renderers;

    public ClauseRendererRegistry(IEnumerable<IClauseRenderer> renderers)
    {
        _renderers = renderers.ToDictionary(renderer => renderer.ClauseKind);
    }

    public IClauseRenderer GetRenderer(IQueryClause clause)
    {
        if (!_renderers.TryGetValue(clause.Kind, out var renderer))
        {
            throw new InvalidOperationException($"No renderer registered for {clause.Kind}.");
        }

        return renderer;
    }
}