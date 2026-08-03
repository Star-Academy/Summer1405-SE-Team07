using System.Collections.Generic;

namespace QueryLib.Compilers
{
    // SRP: this type only carries the SQL text and its parameter bindings.
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

    // ISP: consumers depend on one focused operation instead of database-specific APIs.
    // DIP: callers compile queries through this abstraction rather than concrete compilers.
    public interface ICompiler
    {
        CompiledQuery Compile(Query query);
    }
}
