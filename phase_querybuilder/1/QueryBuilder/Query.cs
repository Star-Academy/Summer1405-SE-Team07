namespace QueryBuilder;

public class Query
{
    private string _table = string.Empty;
    private string[] _columns = [];

    private readonly List<(string Column, object Value)> _conditions = new();

    public Query From(string table)
    {
        _table = table;
        return this;
    }

    public Query Select(params string[] columns)
    {
        _columns = columns;
        return this;
    }

    public Query Where(string column, object value)
    {
        _conditions.Add((column, value));
        return this;
    }

    public override string ToString()
    {
        string sql =
            $"SELECT {string.Join(", ", _columns)} FROM {_table}";

        if (_conditions.Count > 0)
        {
            string whereClause = string.Join(
                " AND ",
                _conditions.Select(condition =>
                    $"{condition.Column} = {FormatValue(condition.Value)}")
            );

            sql += $" WHERE {whereClause}";
        }

        return sql;
    }

    private string FormatValue(object value)
    {
        if (value is string text)
        {
            return $"'{text}'";
        }

        if (value is bool boolean)
        {
            return boolean ? "TRUE" : "FALSE";
        }

        return value.ToString() ?? "NULL";
    }
}