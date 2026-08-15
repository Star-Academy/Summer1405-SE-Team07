namespace QueryLib.Demo.Execution.Abstractions;

public interface IQueryExecutionDependencyFactory
{
    QueryExecutionDependencies Create(DbConfiguration configuration);
}
