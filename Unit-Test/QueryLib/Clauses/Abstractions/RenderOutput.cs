namespace QueryLib.Clauses.Abstractions;

public sealed record RenderOutput(
    string Sql,
    IReadOnlyCollection<object?> Bindings);
