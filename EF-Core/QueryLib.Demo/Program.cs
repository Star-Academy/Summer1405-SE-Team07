using Microsoft.Extensions.Configuration;
using QueryLib.Demo.EFCore;

namespace QueryLib.Demo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");
        
        await EfCoreDemo.RunAsync(connectionString!);
    }
}