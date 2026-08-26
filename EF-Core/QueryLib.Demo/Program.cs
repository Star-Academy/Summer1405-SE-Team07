using QueryLib.Demo.EFCore;

namespace QueryLib.Demo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await EfCoreDemo.RunAsync();
    }
}