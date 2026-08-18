using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Renderers.Abstractions;

public interface IClauseRenderer
{
    Type ClauseType { get; }

    RenderOutput Render(
        IQueryClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings);
}








