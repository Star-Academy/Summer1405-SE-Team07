namespace QueryLib.Demo.Execution.Abstractions;

public interface IQueryExecutionReporter
{
    void ReportStarted(DbProvider provider);

    Task ReportSucceededAsync(DbProvider provider, QueryExecutionResult executionResult);

    void ReportFailed(DbProvider provider, Exception exception);

    void ReportCompleted();
}
