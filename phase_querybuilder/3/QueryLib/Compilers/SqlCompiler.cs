using System.Collections.Generic;
using System.Linq;
using QueryLib.Dialects;
using QueryLib.Interfaces;
using QueryLib.Clauses;
using QueryLib.Clauses.Interfaces;
namespace QueryLib.Compilers;

public sealed class SqlCompiler : ICompiler
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;
    private readonly IValueBinder _binder;

    public SqlCompiler(
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IValueBinder binder)
    {
        _quoter = quoter;
        _placeholders = placeholders;
        _binder = binder;
    }

    public CompiledQuery Compile(Query query)
    {
        var rawBindings = new List<object?>();

        var selectColumns = query.Columns.Count > 0
            ? string.Join(", ", query.Columns.Select(_quoter.Quote))
            : "*";

        var sqlParts = new List<string>
        {
            $"SELECT {selectColumns}",
            $"FROM {_quoter.Quote(query.Table)}"
        };

        foreach (var clause in query.Clauses.OrderBy(c => c.Order))
        {
            var rendered = clause.Render(_quoter, _placeholders, rawBindings);
            if (!string.IsNullOrEmpty(rendered))
                sqlParts.Add(rendered);
        }

        var boundValues = rawBindings.Select(_binder.Bind).ToList();
        return new CompiledQuery(string.Join(" ", sqlParts), boundValues);
    }
}
