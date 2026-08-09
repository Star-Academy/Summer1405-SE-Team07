
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Clauses.Abstractions;

public interface IQueryClause
{
    int Order { get; }

    RenderOutput Render(
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        List<object?> bindings);
}
