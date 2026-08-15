using QueryLib.Dialects.Abstractions;
using QueryLib.Compilers.Abstractions;

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
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
        _binder = binder ?? throw new ArgumentNullException(nameof(binder));
        _placeholders =  placeholders ?? throw new ArgumentNullException(nameof(placeholders));
    }

    public CompiledQuery Compile(Query query)
    {
        var rawBindings = Array.Empty<object?>();

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
            rawBindings = rendered.Bindings.ToArray();

            if (!string.IsNullOrEmpty(rendered.Sql))
                sqlParts.Add(rendered.Sql);
        }

        var boundValues = rawBindings.Select(_binder.Bind).ToList();
        return new CompiledQuery(string.Join(" ", sqlParts), boundValues);
    }
}
