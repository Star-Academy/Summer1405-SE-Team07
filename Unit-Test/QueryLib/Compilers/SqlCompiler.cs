using QueryLib.Clauses.Abstractions;
using QueryLib.Compilers.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryLib.Compilers;

public sealed class SqlCompiler : ICompiler
{
    private readonly IIdentifierQuoter _quoter;
    private readonly IParameterPlaceholderFactory _placeholders;
    private readonly IValueBinder _binder;
    private readonly ClauseRendererRegistry _rendererRegistry;
    private readonly SelectClauseRenderer _selectClauseRenderer;
    private readonly WhereClauseRenderer _whereClauseRenderer;
    private readonly FromClauseRenderer _fromClauseRenderer;

    public SqlCompiler(
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        IValueBinder binder,
        SelectClauseRenderer selectClauseRenderer,
        WhereClauseRenderer whereClauseRenderer,
        FromClauseRenderer fromClauseRenderer)
    {
        _quoter = quoter ?? throw new ArgumentNullException(nameof(quoter));
        _placeholders = placeholders ?? throw new ArgumentNullException(nameof(placeholders));
        _binder = binder ?? throw new ArgumentNullException(nameof(binder));

        _rendererRegistry = new ClauseRendererRegistry(
        [
            _selectClauseRenderer =  selectClauseRenderer ,
            _fromClauseRenderer = fromClauseRenderer ,
            _whereClauseRenderer = whereClauseRenderer
        ]);
    }

    public CompiledQuery Compile(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var renderedQuery = ClauseRender(query.Clauses);

        var boundValues = renderedQuery.Bindings
            .Select(_binder.Bind)
            .ToList();

        return new CompiledQuery(renderedQuery.Sql, boundValues);
    }

    private RenderOutput ClauseRender(IEnumerable<IQueryClause> clauses)
    {
        IReadOnlyCollection<object?> bindings = Array.Empty<object?>();

        var sqlParts = new List<string>();

        foreach (var clause in clauses.OrderBy(clause => clause.Order))
        {
            var renderer = _rendererRegistry.GetRenderer(clause);

            var output = renderer.Render(
                clause,
                _quoter,
                _placeholders,
                bindings);
            
            bindings = output.Bindings;

            if (!string.IsNullOrWhiteSpace(output.Sql))
            {
                sqlParts.Add(output.Sql);
            }
        }

        return new RenderOutput(string.Join(" ", sqlParts), bindings);
    }
}