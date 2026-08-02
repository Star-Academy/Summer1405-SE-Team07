using QueryLib;

var query = new Query()
    .From("Students")
    .Select("Id", "Name")
    .Where("IsMale", true)
    .Where("Age", 20);

Console.WriteLine();

Console.WriteLine("Simple display:");
Console.WriteLine(query);
Console.WriteLine();

List<KeyValuePair<string, object>> parameters;
string sql = query.ToSql(out parameters);

Console.WriteLine("Parameterized SQL: ");
Console.WriteLine(sql);
Console.WriteLine();

Console.WriteLine("Parameters: ");
foreach (var p in parameters)
{
    Console.WriteLine($"  {p.Key} = {p.Value}");
}

Console.WriteLine();
