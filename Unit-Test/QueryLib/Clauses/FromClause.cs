using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses;

public sealed class FromClause : IQueryClause
{
    public required string Table { get; init; }
    public int Order => 1;
    public ClauseKind Kind => ClauseKind.From;
}