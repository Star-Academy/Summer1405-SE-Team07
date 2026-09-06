using System.Data.Common;

namespace QueryLib.IntegrationTest.Infrastructure;

internal static class CompiledQueryRowReader
{
    public static async Task<List<Dictionary<string, object?>>> ReadAllAsync(DbDataReader reader)
    {
        var rows = new List<Dictionary<string, object?>>();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            rows.Add(row);
        }

        return rows;
    }
}
