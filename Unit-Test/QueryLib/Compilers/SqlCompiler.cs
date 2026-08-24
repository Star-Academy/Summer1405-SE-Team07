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
    private readonly IReadOnlyDictionary<DbProvider, ClauseRendererRegistry> _rendererRegistries;

    public SqlCompiler(
        IValueBinderFactory valueBinderFactory,
        IReadOnlyDictionary<DbProvider, ClauseRendererRegistry> rendererRegistries)
    {
        _valueBinderFactory = valueBinderFactory ?? throw new ArgumentNullException(nameof(valueBinderFactory));
        _rendererRegistries = rendererRegistries ?? throw new ArgumentNullException(nameof(rendererRegistries));
    }

    public CompiledQuery Compile(Query query, DbProvider dbProvider)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (!_rendererRegistries.TryGetValue(dbProvider, out var rendererRegistry))
        {
            throw new ArgumentOutOfRangeException(
                nameof(dbProvider),
                dbProvider,
                "Unsupported database provider.");
        }

        var renderedQuery = ClauseRender(query.Clauses, rendererRegistry);

        var boundValues = renderedQuery.Bindings
            .Select(_valueBinderFactory.GetBinder(dbProvider).Bind)
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