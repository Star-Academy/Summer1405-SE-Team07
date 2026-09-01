using System.Data;

namespace asp_webapi.Services.Abstractions;

public interface IDbConnectionProvider
{
    IDbConnection CreateConnection();
}