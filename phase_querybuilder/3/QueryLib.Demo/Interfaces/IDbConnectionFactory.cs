using System.Data.Common;
namespace QueryLib.Demo.Interfaces;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync();
}
