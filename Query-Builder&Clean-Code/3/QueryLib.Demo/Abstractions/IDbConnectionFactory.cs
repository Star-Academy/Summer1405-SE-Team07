using System.Data.Common;
namespace QueryLib.Demo.Abstractions;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync();
}
