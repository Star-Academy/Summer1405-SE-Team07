using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers;

public class WhereClauseRenderer : IClauseRenderer
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;

    public DbProvider Provider { get; }

    public WhereClauseRenderer(
        DbProvider provider,
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders)
    {
        Provider = provider;
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
        _placeholders = placeholders ?? throw new ArgumentNullException(nameof(placeholders));
    }

    public WhereClauseRenderer(IIdentifierQuoter quoter, IParameterPlaceholderFactory placeholders)
        : this(DbProvider.SqlServer, quoter, placeholders)
    {
    }

    public ClauseKind ClauseKind => ClauseKind.Where;

    public RenderOutput Render(IQueryClause clause)
    {
        var whereClause = (WhereClause)clause;

        if (!whereClause.HasConditions)
        {
            return new RenderOutput(string.Empty, Array.Empty<object?>());
        }

        var localBindings = new List<object?>();
        var parts = new List<string>();

        foreach (var condition in whereClause.Conditions)
        {
            localBindings.Add(condition.Value);
            var placeholder = _placeholders.MakePlaceholder(localBindings.Count);
            parts.Add($"{_quoter.Quote(condition.Column)} = {placeholder}");
        }

        return new RenderOutput("WHERE " + string.Join(" AND ", parts), localBindings);
    }
}