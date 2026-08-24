using System.Data.Common;

namespace QueryLib.Demo.Abstractions;

public interface IDbConnectionFactory
{
    DbConnection Create(string connectionString);
}