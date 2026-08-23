using System.Data.Common;

namespace QueryLib.Demo.Execution.Abstractions;

public interface IQueryExecutionDependencyFactory
{
    DbConnection Create(DbConfiguration configuration);
}
