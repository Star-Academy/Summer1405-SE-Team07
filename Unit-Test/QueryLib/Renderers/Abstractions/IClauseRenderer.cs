using QueryLib.Clauses.Abstractions;

namespace QueryLib.Renderers.Abstractions;

public interface IClauseRenderer
{
    DbProvider Provider { get; }
    ClauseKind ClauseKind { get; }

    RenderOutput Render(IQueryClause clause);
}