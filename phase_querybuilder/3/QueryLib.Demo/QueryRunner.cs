using Npgsql;
using Microsoft.Data.SqlClient;
using QueryLib.Compilers;

namespace QueryLib.Demo
{

    
    public static class QueryRunner
    {
        public static async Task RunOnPostgresAsync(CompiledQuery compiled, string connectionString)
        {
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var cmd = dataSource.CreateCommand(compiled.Sql);

            foreach (var value in compiled.Bindings)
                cmd.Parameters.AddWithValue(value ?? DBNull.Value);

            await using var reader = await cmd.ExecuteReaderAsync();

            Console.WriteLine("--- PostgreSQL results ---");
            await PrintRowsAsync(reader);
        }

        public static async Task RunOnSqlServerAsync(CompiledQuery compiled, string connectionString)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand(compiled.Sql, connection);

            for (int i = 0; i < compiled.Bindings.Count; i++)
                cmd.Parameters.AddWithValue("@p" + i, compiled.Bindings[i] ?? DBNull.Value);

            await using var reader = await cmd.ExecuteReaderAsync();

            Console.WriteLine("--- SQL Server results ---");
            await PrintRowsAsync(reader);
        }
        private static async Task PrintRowsAsync(System.Data.Common.DbDataReader reader)
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
}
