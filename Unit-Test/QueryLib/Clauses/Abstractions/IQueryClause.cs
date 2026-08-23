namespace QueryLib.Clauses.Abstractions;

public interface IQueryClause
{
    int Order { get; }
    ClauseKind Kind { get; }
}