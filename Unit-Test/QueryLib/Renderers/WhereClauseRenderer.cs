using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public sealed class WhereClauseRenderer : IClauseRenderer
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;

    public WhereClauseRenderer(IIdentifierQuoter quoter, IParameterPlaceholderFactory placeholders)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
        _placeholders = placeholders ?? throw new ArgumentNullException(nameof(placeholders));
    }

    public Type ClauseType => typeof(WhereClause);

    public RenderOutput Render(IQueryClause clause, IReadOnlyCollection<object?> bindings)
    {
        var whereClause = (WhereClause)clause;
        var mutableBindings = bindings.ToList();

        if (!whereClause.HasConditions)
        {
            return new RenderOutput(string.Empty, mutableBindings);
        }

        var parts = new List<string>();

        foreach (var condition in whereClause.Conditions)
        {
            mutableBindings.Add(condition.Value);
            var placeholder = _placeholders.MakePlaceholder(mutableBindings.Count);
            parts.Add($"{_quoter.Quote(condition.Column)} = {placeholder}");
        }

        return new RenderOutput("WHERE " + string.Join(" AND ", parts), mutableBindings);
    }
}