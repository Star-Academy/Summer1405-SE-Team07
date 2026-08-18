using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public abstract class ClauseRenderer<TClause>
    : IClauseRenderer, IClauseRenderer<TClause>
    where TClause : IQueryClause
{
    public Type ClauseType => typeof(TClause);

    public abstract RenderOutput Render(
        TClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings);
    
    RenderOutput IClauseRenderer.Render(
        IQueryClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings)
    {
        return Render(
            (TClause)clause,
            quoter,
            placeholders,
            bindings);
    }
}



