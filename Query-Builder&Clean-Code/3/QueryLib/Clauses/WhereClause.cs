using QueryLib.Dialects.Abstractions;
using QueryLib.Clauses.Abstractions;

namespace QueryLib.Clauses
{
    public sealed class WhereClause : IQueryClause
    {
        private readonly List<Condition> _conditions = new();

        public int Order => 20;

        public bool HasConditions => _conditions.Count > 0;

        public void Add(Condition condition) => _conditions.Add(condition);

        public RenderOutput Render(
            IIdentifierQuoter quoter,
            IParameterPlaceholderFactory placeholders,
            IReadOnlyCollection<object?> bindings)
        {
            var mutableBindings = bindings.ToList();

            if (!HasConditions)
                return new RenderOutput(string.Empty, mutableBindings);

            var parts = new List<string>();
            foreach (var condition in _conditions)
            {
                mutableBindings.Add(condition.Value);
                var placeholder = placeholders.MakePlaceholder(mutableBindings.Count);
                parts.Add($"{quoter.Quote(condition.Column)} = {placeholder}");
            }

            return new RenderOutput(
                "WHERE " + string.Join(" AND ", parts),
                mutableBindings);
        }
    }
}
