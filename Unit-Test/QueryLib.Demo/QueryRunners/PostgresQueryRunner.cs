using System.Data.Common;
using QueryLib.Compilers;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.QueryRunners;

public sealed class PostgresQueryRunner : IQueryRunner
{
    public async Task<QueryResult> RunAsync(
        CompiledQuery query,
        DbConnection connection,
        DbTransaction? transaction = null)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException(
                "The provided connection is not open. Ensure the connection is opened before executing the query.");
        }

        await using var command = connection.CreateCommand();
        command.CommandText = query.Sql;
        command.Transaction = transaction;

        foreach (var value in query.Bindings)
        {
            var param = command.CreateParameter();
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
        }

        await using var reader = await command.ExecuteReaderAsync();

        var columnNames = new List<string>();
        var rows = new List<Dictionary<string, object?>>();

        for (int column = 0; column < reader.FieldCount; column++)
        {
            columnNames.Add(reader.GetName(column));
        }

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();

            for (int column = 0; column < reader.FieldCount; column++)
            {
                var value = reader.GetValue(column);
                row[reader.GetName(column)] = value == DBNull.Value ? null : value;
            }

            rows.Add(row);
        }

        return new QueryResult
        {
            ColumnNames = columnNames,
            Rows = rows
        };
    }
}

