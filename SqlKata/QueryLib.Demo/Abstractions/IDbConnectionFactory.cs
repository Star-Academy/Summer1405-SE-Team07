using System.Data.Common;

namespace QueryLib.Demo.Abstractions;

public interface IDbConnectionFactory
{
    DbProvider Provider { get; }
    DbConnection Create(string connectionString);
}