using QueryLib.Clauses.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class ClauseRendererRegistry
{
    private readonly Dictionary<Type, IClauseRenderer> _renderers;

    public ClauseRendererRegistry(IEnumerable<IClauseRenderer> renderers)
    {
        _renderers = renderers.ToDictionary(renderer => renderer.ClauseType);
    }

    public IClauseRenderer GetRenderer(IQueryClause clause)
    {
        var clauseType = clause.GetType();

        if (!_renderers.TryGetValue(clauseType, out var renderer))
        {
            throw new InvalidOperationException($"No renderer registered for {clauseType.Name}.");
        }

        return renderer;
    }
}