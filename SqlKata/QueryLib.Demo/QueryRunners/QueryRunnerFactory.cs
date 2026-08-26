using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.QueryRunners;

public sealed class QueryRunnerFactory : IQueryRunnerFactory
{
    private readonly Dictionary<DbProvider, IQueryRunner> _runners;

    public QueryRunnerFactory(IEnumerable<IQueryRunner> runners)
    {
        _runners = runners.ToDictionary(r => r.Provider);
    }

    public IQueryRunner GetRunner(DbProvider provider)
    {
        if (_runners.TryGetValue(provider, out var runner))
        {
            return runner;
        }
        throw new KeyNotFoundException($"No query runner registered for provider {provider}.");
    }
}

