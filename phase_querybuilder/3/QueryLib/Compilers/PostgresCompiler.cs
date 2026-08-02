using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryLib.Compilers
{
    public class PostgresCompiler : ICompiler
    {
        public CompiledQuery Compile(Query query)
        {
            var bindings = new List<object?>();
            var sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append(query.Columns.Count > 0
                ? string.Join(", ", query.Columns.Select(Quote))
                : "*");

            sb.Append(" FROM ");
            sb.Append(Quote(query.Table));

            if (query.Conditions.Count > 0)
            {
                sb.Append(" WHERE ");

                var clauses = new List<string>();
                for (int i = 0; i < query.Conditions.Count; i++)
                {
                    var condition = query.Conditions[i];
                    string placeholder = "$" + (i + 1); // Postgres placeholders are 1-based

                    clauses.Add($"{Quote(condition.Column)} = {placeholder}");
                    bindings.Add(condition.Value);
                }

                sb.Append(string.Join(" AND ", clauses));
            }

            return new CompiledQuery(sb.ToString(), bindings);
        }
        private static string Quote(string identifier) => $"\"{identifier}\"";
    }
}
