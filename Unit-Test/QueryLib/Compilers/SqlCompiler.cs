using QueryLib.Clauses.Abstractions;
using QueryLib.Compilers.Abstractions;
using QueryLib.Dialects.Abstractions;
using QueryLib.Renderers;
using QueryLib.Renderers.Abstractions;

namespace QueryLib.Compilers;

public sealed class SqlCompiler : ICompiler
{
    private readonly IValueBinderFactory _valueBinderFactory;
    private readonly IClauseRendererRegistryFactory _registryFactory;

    public SqlCompiler(
        IValueBinderFactory valueBinderFactory,
        IClauseRendererRegistryFactory registryFactory)
    {
        _valueBinderFactory = valueBinderFactory ?? throw new ArgumentNullException(nameof(valueBinderFactory));
        _registryFactory = registryFactory ?? throw new ArgumentNullException(nameof(registryFactory));
    }

    public CompiledQuery Compile(Query query, DbProvider dbProvider)
    {
        ArgumentNullException.ThrowIfNull(query);

        var rendererRegistry = _registryFactory.GetRegistry(dbProvider);
        var renderedQuery = ClauseRender(query.Clauses, rendererRegistry);

        var binder = _valueBinderFactory.GetBinder(dbProvider);
        var boundValues = renderedQuery.Bindings
            .Select(binder.Bind)
            .ToList();

        return new CompiledQuery(renderedQuery.Sql, boundValues);
    }

    private static RenderOutput ClauseRender(IEnumerable<IQueryClause> clauses, ClauseRendererRegistry rendererRegistry)
    {
        var allBindings = new List<object?>();
        var sqlParts = new List<string>();

        foreach (var clause in clauses.OrderBy(clause => clause.Order))
        {
            var renderer = rendererRegistry.GetRenderer(clause);
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