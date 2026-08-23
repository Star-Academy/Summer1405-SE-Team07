using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses;

public sealed class WhereClause : IQueryClause
{
    public required IReadOnlyCollection<Condition> Conditions { get; init; }
    public int Order => 2;
    public ClauseKind Kind => ClauseKind.Where;
    public bool HasConditions => Conditions.Count > 0;
}