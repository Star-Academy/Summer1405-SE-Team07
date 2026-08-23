using QueryLib.Clauses.Abstractions;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo;
using QueryLib.Demo.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;

namespace QueryLib.Compilers;

public sealed class SqlCompiler : ICompiler
{
    private readonly IValueBinderFactory _valueBinderFactory;
    private readonly ClauseRendererRegistry _rendererRegistry;

    public SqlCompiler(
        IValueBinderFactory valueBinderFactory,
        ClauseRendererRegistry rendererRegistry)
    {
        _valueBinderFactory = valueBinderFactory ?? throw new ArgumentNullException(nameof(valueBinderFactory));
        _rendererRegistry = rendererRegistry ?? throw new ArgumentNullException(nameof(rendererRegistry));
    }

    public CompiledQuery Compile(Query query, DbProvider dbProvider)
    {
        ArgumentNullException.ThrowIfNull(query);

        var renderedQuery = ClauseRender(query.Clauses);

        var boundValues = renderedQuery.Bindings
            .Select(_valueBinderFactory.GetBinder("postgres").Bind)
            .ToList();

        return new CompiledQuery(renderedQuery.Sql, boundValues);
    }

    public DbProvider DbProvider => DbProvider.SqlServer;

    private RenderOutput ClauseRender(IEnumerable<IQueryClause> clauses)
    {
        var allBindings = new List<object?>();
        var sqlParts = new List<string>();

        foreach (var clause in clauses.OrderBy(clause => clause.Order))
        {
            var renderer = _rendererRegistry.GetRenderer(clause);
            var output = renderer.Render(clause);

            allBindings.AddRange(output.Bindings);

            if (!string.IsNullOrWhiteSpace(output.Sql))
            {
                sqlParts.Add(output.Sql);
            }
        }

        return new RenderOutput(string.Join(" ", sqlParts), allBindings);
    }
}