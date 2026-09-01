using asp_webapi.Exceptions;
using SqlKata.Execution;
using asp_webapi.Services.Abstractions;
using SqlKata.Compilers;
using SqlKata.Execution;
namespace asp_webapi.Services;

public class DatabaseFactory : IDatabaseFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public QueryFactory CreateQueryFactory(string? dbType)
    {
        if (string.IsNullOrWhiteSpace(dbType))
        {
            throw new UnsupportedDatabaseTypeException(dbType);
        }

        var normalized = dbType.Trim().ToLowerInvariant();

        var connectionProvider = _serviceProvider.GetKeyedService<IDbConnectionProvider>(normalized)
                                 ?? throw new UnsupportedDatabaseTypeException(dbType);

        var compiler = _serviceProvider.GetKeyedService<Compiler>(normalized)
                       ?? throw new UnsupportedDatabaseTypeException(dbType);

        return new QueryFactory(connectionProvider.CreateConnection(), compiler);
    }
}