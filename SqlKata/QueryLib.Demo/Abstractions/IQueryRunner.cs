using System.Data.Common;
using QueryLib.Compilers;

namespace QueryLib.Demo.Abstractions;

public interface IQueryRunner
{
    DbProvider Provider { get; }
    Task<QueryResult> RunAsync(CompiledQuery query, DbConnection connection, DbTransaction? transaction = null);
}
