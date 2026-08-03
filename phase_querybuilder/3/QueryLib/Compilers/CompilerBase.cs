using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryLib.Compilers
{
    // SRP: this class owns the database-independent query compilation algorithm.
    // OCP: new SQL dialects extend the protected hooks without changing this algorithm.
    public abstract class CompilerBase : ICompiler
    {
        public CompiledQuery Compile(Query query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var bindings = new List<object?>();
            var sql = new StringBuilder();

            sql.Append("SELECT ");
            sql.Append(query.Columns.Count > 0
                ? string.Join(", ", query.Columns.Select(QuoteIdentifier))
                : "*");

            sql.Append(" FROM ");
            sql.Append(QuoteIdentifier(query.Table));

            if (query.Conditions.Count > 0)
            {
                sql.Append(" WHERE ");

                var clauses = new List<string>();
                for (int i = 0; i < query.Conditions.Count; i++)
                {
                    var condition = query.Conditions[i];
                    clauses.Add($"{QuoteIdentifier(condition.Column)} = {CreatePlaceholder(i)}");
                    bindings.Add(PrepareBinding(condition.Value));
                }

                sql.Append(string.Join(" AND ", clauses));
            }

            return new CompiledQuery(sql.ToString(), bindings);
        }

        protected abstract string QuoteIdentifier(string identifier);
        protected abstract string CreatePlaceholder(int index);

        protected virtual object? PrepareBinding(object? value) => value;
    }
}
