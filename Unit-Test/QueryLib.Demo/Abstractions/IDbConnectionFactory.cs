using System.Data.Common;

namespace QueryLib.Demo.Abstractions;

public interface IDbConnectionFactoryProvider
{
    DbConnection CreateConnection(string connectionString);
}