using System.Data.Common;
using QueryLib.Compilers;
using QueryLib.Demo.Interfaces;

namespace QueryLib.Demo.QueryRunners;

public sealed class SqlServerQueryRunner : IQueryRunner
{
    public async Task<QueryResult> RunAsync(CompiledQuery query, DbConnection connection, DbTransaction? transaction = null)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("The provided connection is not open. Ensure the connection is opened before executing the query.");  
        }
    
        await using var command = connection.CreateCommand();
        command.CommandText = query.Sql;
        command.Transaction = transaction;

        for (int param = 0; param < query.Bindings.Count; param++)
        {
            var dbParam = command.CreateParameter();
            dbParam.ParameterName = "@p" + param;
            dbParam.Value = query.Bindings[param] ?? DBNull.Value;
            command.Parameters.Add(dbParam);
        }

        await using var reader = await command.ExecuteReaderAsync();

        var result = new QueryResult();

        for (int col = 0; col < reader.FieldCount; col++)
        {
            result.ColumnNames.Add(reader.GetName(col));
        }

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (int col = 0; col < reader.FieldCount; col++)
            {
                var val = reader.GetValue(col);
                row[reader.GetName(col)] = val == DBNull.Value ? null : val;
            }
            result.Rows.Add(row);
        }

        return result;
    }
}
