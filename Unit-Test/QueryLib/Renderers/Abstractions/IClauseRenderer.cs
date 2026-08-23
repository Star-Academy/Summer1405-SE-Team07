using QueryLib.Clauses.Abstractions;

namespace QueryLib.Renderers.Abstractions;

public interface IClauseRenderer
{
    ClauseKind ClauseKind { get; }

    RenderOutput Render(IQueryClause clause);
}