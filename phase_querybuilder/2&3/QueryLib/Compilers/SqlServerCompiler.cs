using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryLib.Compilers
{
    public class SqlServerCompiler : ICompiler
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
                    string placeholder = "@p" + i; // SQL Server placeholders are 0-based

                    clauses.Add($"{Quote(condition.Column)} = {placeholder}");
                    bindings.Add(ToSqlServerValue(condition.Value));
                }

                sb.Append(string.Join(" AND ", clauses));
            }

            return new CompiledQuery(sb.ToString(), bindings);
        }

        private static string Quote(string identifier) => $"[{identifier}]";

        private static object? ToSqlServerValue(object? value)
        {
            if (value is bool b)
                return b ? 1 : 0;

            return value;
        }
    }
}
