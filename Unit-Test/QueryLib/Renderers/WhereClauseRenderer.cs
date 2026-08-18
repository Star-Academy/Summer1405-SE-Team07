using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;

namespace QueryLib.Renderers;

public sealed class WhereClauseRenderer
    : ClauseRenderer<WhereClause>
{
    public override RenderOutput Render(
        WhereClause clause,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IReadOnlyCollection<object?> bindings)
    {
        var mutableBindings = bindings.ToList();

        if (!clause.HasConditions)
        {
            return new RenderOutput(
                string.Empty,
                mutableBindings);
        }
        
        
        
        
        
        var parts = new List<string>();

        foreach (var condition in clause.Conditions)
        {
            mutableBindings.Add(condition.Value);

            var placeholder =
                placeholders.MakePlaceholder(mutableBindings.Count);

            parts.Add(
                $"{quoter.Quote(condition.Column)} = {placeholder}");
        }

        return new RenderOutput(
            "WHERE " + string.Join(" AND ", parts),
            mutableBindings);
    }
}