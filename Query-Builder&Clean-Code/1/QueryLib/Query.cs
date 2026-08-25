using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryLib
{
    
    public class Query
    {
        private string _table;
        private List<string> _columns = new List<string>();
        private List<KeyValuePair<string, object>> _conditions = new List<KeyValuePair<string, object>>();

        public Query From(string table)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Table name cannot be NULL.", nameof(table));

            _table = table;
            return this;
        }

        public Query Select(params string[] columns)
        {
            if (columns != null && columns.Length > 0)
                _columns.AddRange(columns);

            return this;
        }

        public Query Where(string column, object value)
        {
            if (string.IsNullOrWhiteSpace(column))
                throw new ArgumentException("Table name cannot be NULL.", nameof(column));

            _conditions.Add(new KeyValuePair<string, object>(column, value));
            return this;
        }

        public string ToSql(out List<KeyValuePair<string, object>> parameters)
        {
            if (string.IsNullOrWhiteSpace(_table))
                throw new InvalidOperationException("Before building query FROM(..) needs to be called.");

            parameters = new List<KeyValuePair<string, object>>();

            var sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append(_columns.Count > 0 ? string.Join(", ", _columns) : "*");

            sb.Append(" FROM ");
            sb.Append(_table);

            if (_conditions.Count > 0)
            {
                sb.Append(" WHERE ");

                var clauses = new List<string>();
                for (int i = 0; i < _conditions.Count; i++)
                {
                    string paramName = "@p" + i;
                    clauses.Add($"{_conditions[i].Key} = {paramName}");
                    parameters.Add(new KeyValuePair<string, object>(paramName, _conditions[i].Value));
                }

                sb.Append(string.Join(" AND ", clauses));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            List<KeyValuePair<string, object>> parameters;
            string sql = ToSql(out parameters);

            foreach (var p in parameters)
            {
                string valueStr = FormatValueForDisplay(p.Value);
                sql = sql.Replace(p.Key, valueStr);
            }

            return sql;
        }

        private static string FormatValueForDisplay(object value)
        {
            if (value == null) return "NULL";
            if (value is bool b) return b ? "1" : "0";
            if (value is string s) return $"'{s}'";
            return value.ToString();
        }
    }
}
