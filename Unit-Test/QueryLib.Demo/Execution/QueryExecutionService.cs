using QueryLib.Demo.Execution.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed class QueryExecutionService
{
    private readonly IDatabaseQueryExecutor _queryExecutor;
    private readonly IQueryExecutionReporter _reporter;

    public QueryExecutionService(
        IDatabaseQueryExecutor queryExecutor,
        IQueryExecutionReporter reporter)
    {
        _queryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
    }

    public async Task ExecuteAsync(Query query, IEnumerable<DbConfiguration> configurations)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(configurations);

        foreach (var configuration in configurations)
        {
            _reporter.ReportStarted(configuration.Provider);

            try
            {
                var executionResult = await _queryExecutor.ExecuteAsync(query, configuration);
                await _reporter.ReportSucceededAsync(configuration.Provider, executionResult);
            }
            catch (Exception exception)
            {
                _reporter.ReportFailed(configuration.Provider, exception);
            }
            finally
            {
                _reporter.ReportCompleted();
            }
        }
    }
}
