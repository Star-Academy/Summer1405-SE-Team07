using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Renderers;

public sealed class FromClauseRenderer
    : ClauseRenderer<FromClause>
{
    public override RenderOutput Render(
        FromClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings)
    {
        return new RenderOutput(
            $"FROM {quoter.Quote(clause.Table)}", bindings);
    }
}