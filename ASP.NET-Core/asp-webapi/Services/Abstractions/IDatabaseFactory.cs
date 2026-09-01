using SqlKata.Execution;

namespace asp_webapi.Services.Abstractions;

public interface IDatabaseFactory
{
    QueryFactory CreateQueryFactory(string? dbType);
}