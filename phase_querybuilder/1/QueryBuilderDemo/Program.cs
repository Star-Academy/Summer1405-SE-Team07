using QueryBuilder;

var query = new Query()
    .From("Students")
    .Select("Id", "Name")
    .Where("IsMale", true)
    .Where("Age", 20)
    .Where("Name", "Ali");

Console.WriteLine(query);

