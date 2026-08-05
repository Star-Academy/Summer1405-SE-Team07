using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace QueryLib.Demo;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync();
}