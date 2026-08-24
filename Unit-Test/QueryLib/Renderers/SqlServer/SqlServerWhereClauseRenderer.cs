using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;
using QueryLib.Dialects.SqlServer;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Renderers.SqlServer;

public sealed class SqlServerWhereClauseRenderer : IClauseRenderer
{
    private readonly SqlServerIdentifierQuoter _quoter;
    private readonly SqlServerParameterPlaceholderFactory _placeholders;

    public DbProvider Provider => DbProvider.SqlServer;
    public ClauseKind ClauseKind => ClauseKind.Where;

    public SqlServerWhereClauseRenderer(
        SqlServerIdentifierQuoter quoter,
        SqlServerParameterPlaceholderFactory placeholders)
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

