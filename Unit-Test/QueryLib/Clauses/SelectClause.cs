using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses;

public sealed class SelectClause : IQueryClause
{
    public required IReadOnlyCollection<string> Columns { get; init; }
    public int Order => 0;
    public ClauseKind Kind => ClauseKind.Select;
}