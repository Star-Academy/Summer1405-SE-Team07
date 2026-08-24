using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.Postgres;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.Postgres;

public sealed class PostgresWhereClauseRenderer : IClauseRenderer
{
    private readonly PostgresIdentifierQuoter _quoter;
    private readonly PostgresParameterPlaceholderFactory _placeholders;

    public DbProvider Provider => DbProvider.PostgreSql;
    public ClauseKind ClauseKind => ClauseKind.Where;

    public PostgresWhereClauseRenderer(
        PostgresIdentifierQuoter quoter,
        PostgresParameterPlaceholderFactory placeholders)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
        _placeholders = placeholders ?? throw new ArgumentNullException(nameof(placeholders));
    }

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

