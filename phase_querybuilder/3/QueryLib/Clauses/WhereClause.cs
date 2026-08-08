using QueryLib.Dialects; 
using QueryLib.Clauses.Interfaces;

namespace QueryLib.Clauses
{
    public sealed class WhereClause : IQueryClause
    {
        private readonly List<Condition> _conditions = new();

        public int Order => 20;

        public bool HasConditions => _conditions.Count > 0;

        public void Add(Condition condition) => _conditions.Add(condition);

        public string Render(
            IIdentifierQuoter quoter,
            IParameterPlaceholderFactory placeholders,
            List<object?> bindings)
        {
            if (!HasConditions)
                return string.Empty;

            var parts = new List<string>();
            foreach (var condition in _conditions)
            {
                bindings.Add(condition.Value);
                var placeholder = placeholders.MakePlaceholder(bindings.Count);
                parts.Add($"{quoter.Quote(condition.Column)} = {placeholder}");
            }

            return "WHERE " + string.Join(" AND ", parts);
        }
    }
}