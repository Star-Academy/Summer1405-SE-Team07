using QueryLib.Clauses;
using QueryLib.Clauses.Abstractions;



namespace QueryLib;


public sealed class Query
    {
        private string? _table;
        private readonly List<string> _columns = new();
        private readonly List<IQueryClause> _clauses = new();

        private WhereClause? _whereClause;

        public string Table => _table
            ?? throw new InvalidOperationException("From(...) must be called before compiling the query.");

        public IReadOnlyCollection<string> Columns => _columns;
        public IReadOnlyCollection<IQueryClause> Clauses => _clauses;

        public Query From(string table)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Table name cannot be empty.", nameof(table));

            _table = table;
            return this;
        }

        public Query Select(params string[] columns)
        {
            if (columns != null && columns.Length > 0)
                _columns.AddRange(columns);

            return this;
        }

        public Query Where(string column, object? value)
        {
            _whereClause ??= AddNew(new WhereClause());
            _whereClause.Add(new Condition(column, value));
            return this;
        }
        
        public Query AddClause(IQueryClause clause)
        {
            _clauses.Add(clause);
            return this;
        }

        private T AddNew<T>(T clause) where T : IQueryClause
        {
            _clauses.Add(clause);
            return clause;
        }
}
