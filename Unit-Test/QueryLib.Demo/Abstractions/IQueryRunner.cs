using QueryLib.Compilers;
using System.Data.Common;

namespace QueryLib.Demo.Abstractions;

public interface IQueryRunner
{
    Task<QueryResult> RunAsync(CompiledQuery query, DbConnection connection, DbTransaction? transaction = null);
}
