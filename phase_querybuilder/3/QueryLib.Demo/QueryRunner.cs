using Microsoft.Data.SqlClient;
using Npgsql;
using QueryLib.Compilers;
using System.Data.Common;

namespace QueryLib.Demo
{
    // ISP: callers depend on one focused query-execution operation.
    // DIP: callers use this abstraction instead of a database-specific runner.
    public interface IQueryRunner
    {
        Task RunAsync(CompiledQuery query, string connectionString);
    }

    // SRP: shared result-row formatting is implemented once for every runner.
    public abstract class QueryRunnerBase : IQueryRunner
    {
        public abstract Task RunAsync(CompiledQuery query, string connectionString);

        protected static async Task PrintRowsAsync(DbDataReader reader)
        {
            while (await reader.ReadAsync())
            {
                var values = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    values.Add($"{reader.GetName(i)}={reader.GetValue(i)}");

                Console.WriteLine(string.Join(", ", values));
            }

            Console.WriteLine();
        }
    }

    // SRP: this runner contains only PostgreSQL-specific execution behavior.
    // LSP/OCP: it can replace any IQueryRunner without changing its callers.
    public sealed class PostgresQueryRunner : QueryRunnerBase
    {
        public override async Task RunAsync(CompiledQuery query, string connectionString)
        {
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var command = dataSource.CreateCommand(query.Sql);

            foreach (var value in query.Bindings)
                command.Parameters.AddWithValue(value ?? DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync();

            Console.WriteLine("--- PostgreSQL results ---");
            await PrintRowsAsync(reader);
        }
    }

    // SRP: this runner contains only SQL Server-specific execution behavior.
    // LSP/OCP: it can replace any IQueryRunner without changing its callers.
    public sealed class SqlServerQueryRunner : QueryRunnerBase
    {
        public override async Task RunAsync(CompiledQuery query, string connectionString)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(query.Sql, connection);

            for (int i = 0; i < query.Bindings.Count; i++)
                command.Parameters.AddWithValue("@p" + i, query.Bindings[i] ?? DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync();

            Console.WriteLine("--- SQL Server results ---");
            await PrintRowsAsync(reader);
        }
    }
}
