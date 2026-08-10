using System;
using System.Collections.Generic;

namespace QueryLib
{
    public readonly struct Condition
    {
        public string Column { get; }
        public object? Value { get; }

        public Condition(string column, object? value)
        {
            Column = column;
            Value = value;
        }
    }

    public class Query
    {
        private string? _table;
        private readonly List<string> _columns = new();
        private readonly List<Condition> _conditions = new();

        public string Table => _table
            ?? throw new InvalidOperationException("From(...) must be called before compiling the query.");

        public IReadOnlyList<string> Columns => _columns;
        public IReadOnlyList<Condition> Conditions => _conditions;

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
            if (string.IsNullOrWhiteSpace(column))
                throw new ArgumentException("Column name cannot be empty.", nameof(column));

            _conditions.Add(new Condition(column, value));
            return this;
        }
    }
}
