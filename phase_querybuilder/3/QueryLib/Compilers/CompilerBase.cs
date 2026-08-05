using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueryLib.Interfaces;

namespace QueryLib.Compilers;

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
            int index = 0;
            
            foreach (var condition in query.Conditions)
            {
                clauses.Add($"{QuoteIdentifier(condition.Column)} = {CreatePlaceholder(index)}");
                bindings.Add(PrepareBinding(condition.Value));
                index++;
            }

            sql.Append(string.Join(" AND ", clauses));
        }

        return new CompiledQuery(sql.ToString(), bindings);
    }

    protected abstract string QuoteIdentifier(string identifier);
    protected abstract string CreatePlaceholder(int index);
    protected virtual object? PrepareBinding(object? value) => value;
}

