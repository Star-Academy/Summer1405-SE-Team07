namespace QueryLib.Compilers;
public record CompiledQuery(string Sql, IReadOnlyList<object?> Bindings);
