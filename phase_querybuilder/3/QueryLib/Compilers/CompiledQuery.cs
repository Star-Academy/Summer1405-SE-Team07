using System.Collections.Generic;

namespace QueryLib.Compilers;
public class CompiledQuery
{
    public string Sql { get; }
    public IReadOnlyList<object?> Bindings { get; }
    public CompiledQuery(string sql, IReadOnlyList<object?> bindings)
    {
        Sql = sql;
        Bindings = bindings;
    }
}
