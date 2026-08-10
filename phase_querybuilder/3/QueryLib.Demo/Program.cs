namespace QueryLib.Demo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName", "LastName")
            .Where("IsMale", true);

        var dbConfigurations = new List<DbConfiguration>
        {
            new(DbProvider.PostgreSQL, "Host=localhost;Port=5442;Username=postgres;Password=postgres;Database=mohaymen"),
            new(DbProvider.Sqlserver, "Server=localhost,1433;Database=master;User Id=sa;Password=Your_strong_Password123;Encrypt=False;TrustServerCertificate=True"),
        };

        var service = new Service(dbConfigurations, query);
        await service.RunAsync();
    }
}
