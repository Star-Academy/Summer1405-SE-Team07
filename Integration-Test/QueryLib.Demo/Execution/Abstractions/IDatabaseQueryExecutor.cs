namespace QueryLib.Demo.Execution.Abstractions;

public interface IDatabaseQueryExecutor
{
    Task<QueryExecutionResult> ExecuteAsync(Query query, DbConfiguration configuration);
}
