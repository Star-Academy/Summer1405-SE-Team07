using QueryLib.Clauses.Abstractions;

namespace QueryLib.Renderers.Abstractions;

public interface IClauseRenderer
{
    Type ClauseType { get; }

    RenderOutput Render(IQueryClause clause);
}