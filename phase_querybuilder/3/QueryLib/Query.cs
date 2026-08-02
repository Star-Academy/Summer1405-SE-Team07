using System;
using System.Collections.Generic;

namespace QueryLib
{
    /// <summary>
    /// A single WHERE condition: column name + value to compare with (=).
    /// </summary>
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

    /// <summary>
    /// Query Object.
    /// This class only holds the logical model of a query (table, columns, conditions).
    /// It knows nothing about SQL syntax, quoting styles, or placeholder formats -
    /// that responsibility belongs to ICompiler implementations (PostgresCompiler, SqlServerCompiler, ...).
    /// This is what lets the same Query be compiled to different databases.
    /// </summary>
    public class Query
    {
        private string? _table;
        private readonly List<string> _columns = new();
        private readonly List<Condition> _conditions = new();

        // Read-only views exposed to compilers.
        public string Table => _table
            ?? throw new InvalidOperationException("From(...) must be called before compiling the query.");

        public IReadOnlyList<string> Columns => _columns;
        public IReadOnlyList<Condition> Conditions => _conditions;

        /// <summary>
        /// Sets the source table. Equivalent to FROM Student.
        /// </summary>
        public Query From(string table)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Table name cannot be empty.", nameof(table));

            _table = table;
            return this; // return self => enables method chaining
        }

        /// <summary>
        /// Sets the selected columns. If none are given, compilers should treat it as SELECT *.
        /// </summary>
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
