using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Renderers;

public sealed class SelectClauseRenderer
    : ClauseRenderer<SelectClause>
{
    public override RenderOutput Render(
        SelectClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings)
    {
        var selection = clause.Columns.Count == 0
            ? "*"
            : string.Join(", ", clause.Columns.Select(quoter.Quote));

        return new RenderOutput(
            $"SELECT {selection}", bindings);
    }
}