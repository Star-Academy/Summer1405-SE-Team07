using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses;

public sealed class WhereClause : IQueryClause
{
    private readonly List<Condition> _conditions = new();

    public int Order => 20;

    public bool HasConditions => _conditions.Count > 0;

    public IReadOnlyCollection<Condition> Conditions => _conditions;

    public void Add(Condition condition)
    {
        _conditions.Add(condition);
    }
}