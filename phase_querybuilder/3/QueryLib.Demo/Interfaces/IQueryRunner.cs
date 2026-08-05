using Microsoft.Data.SqlClient;
using Npgsql;
using QueryLib.Compilers;
using System.Data.Common;

namespace QueryLib.Demo
{
public interface IQueryRunner
{
    Task RunAsync(CompiledQuery query, string connectionString);
}
}