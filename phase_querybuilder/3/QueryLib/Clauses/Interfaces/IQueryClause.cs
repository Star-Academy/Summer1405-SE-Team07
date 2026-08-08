

using QueryLib.Dialects;
namespace QueryLib.Clauses.Interfaces;

public interface IQueryClause
{
    int Order { get; }

    string Render(
        IIdentifierQuoter quoter,
        IParameterPlaceholderFactory placeholders,
        List<object?> bindings);
}
