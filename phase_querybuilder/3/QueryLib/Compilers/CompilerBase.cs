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

        CompileSelect(query, sql);
        CompileFrom(query, sql);
        CompileWhere(query, sql, bindings);

        return new CompiledQuery(sql.ToString(), bindings);
    }

    protected virtual void CompileSelect(Query query, StringBuilder sql)
    {
        sql.Append("SELECT ");
        sql.Append(query.Columns.Count > 0
            ? string.Join(", ", query.Columns.Select(QuoteIdentifier))
            : "*");
    }

    protected virtual void CompileFrom(Query query, StringBuilder sql)
    {
        sql.Append(" FROM ");
        sql.Append(QuoteIdentifier(query.Table));
    }

    protected virtual void CompileWhere(Query query, StringBuilder sql, List<object?> bindings)
    {
        if (query.Conditions.Count == 0) return;

        sql.Append(" WHERE ");

        var clauses = new List<string>();
        int index = bindings.Count;

        foreach (var condition in query.Conditions)
        {
            clauses.Add($"{QuoteIdentifier(condition.Column)} = {CreatePlaceholder(index)}");
            bindings.Add(PrepareBinding(condition.Value));
            index++;
        }

        sql.Append(string.Join(" AND ", clauses));
    }

    protected abstract string QuoteIdentifier(string identifier);
    protected abstract string CreatePlaceholder(int index);
    protected virtual object? PrepareBinding(object? value) => value;
}