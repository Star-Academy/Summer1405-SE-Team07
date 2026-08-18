using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Renderers.Abstractions;

public interface IClauseRenderer<in TClause>
    where TClause : IQueryClause
{
    RenderOutput Render(
        TClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings);
}






