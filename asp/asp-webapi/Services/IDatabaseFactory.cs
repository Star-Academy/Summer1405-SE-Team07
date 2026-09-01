using SqlKata.Execution;

namespace asp_webapi.Services;

public interface IDatabaseFactory
{
    QueryFactory CreateQueryFactory(string? dbType);
}