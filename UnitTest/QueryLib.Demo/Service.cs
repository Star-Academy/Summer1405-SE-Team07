using QueryLib.Compilers;
using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;
using QueryLib.Demo.Connections;
using QueryLib.Demo.Printers;
using QueryLib.Demo.QueryRunners;

namespace QueryLib.Demo;

public sealed class Service
{
    private readonly IResultPrinter _printer = new ConsoleResultPrinter();
    private readonly ISqlCompilerFactory _compilerFactory = new SqlCompilerFactory();
    private readonly IReadOnlyList<DbConfiguration> _dbConfigurations;
    private readonly Query _query;

    public Service(IEnumerable<DbConfiguration> dbConfigurations, Query query)
    {
        ArgumentNullException.ThrowIfNull(dbConfigurations);
        ArgumentNullException.ThrowIfNull(query);

        _dbConfigurations = dbConfigurations.ToArray();
        _query = query;
    }

    public async Task RunAsync()
    {
        foreach (var dbConfiguration in _dbConfigurations)
        {
            try
            {
                var (compiler, runner, connectionFactory) = CreateComponents(dbConfiguration);

                Console.WriteLine($"========== {dbConfiguration.provider} ==========");

                var compiledQuery = compiler.Compile(_query);
                PrintCompiled(dbConfiguration.provider.ToString(), compiledQuery);

                await using var connection = await connectionFactory.CreateConnectionAsync();
                await connection.OpenAsync();

                var result = await runner.RunAsync(compiledQuery, connection);
                await _printer.PrintAsync(result, $"{dbConfiguration.provider} Results");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Error executing query on {dbConfiguration.provider}: {exception.Message}");
            }

            Console.WriteLine();
        }
    }

    private (ICompiler Compiler, IQueryRunner Runner, IDbConnectionFactory ConnectionFactory) 
        CreateComponents(DbConfiguration dbConfiguration)
    {
        return dbConfiguration.provider switch
        {
            DbProvider.PostgreSQL => (
                _compilerFactory.Create("postgres"),
                new PostgresQueryRunner(),
                new PostgresConnectionFactory(dbConfiguration.connectionString)),

            DbProvider.Sqlserver => (
                _compilerFactory.Create("sqlserver"),
                new SqlServerQueryRunner(),
                new SqlServerConnectionFactory(dbConfiguration.connectionString)),

            _ => throw new ArgumentOutOfRangeException(
                nameof(dbConfiguration.provider),
                dbConfiguration.provider,
                "Unsupported database provider."),
        };
    }

    private static void PrintCompiled(string label, CompiledQuery result)
    {
        Console.WriteLine($"--- {label} (compiled) ---");
        Console.WriteLine(result.Sql);
        Console.WriteLine("Bindings:");

        for (var index = 0; index < result.Bindings.Count; index++)
        {
            Console.WriteLine($"  [{index}] = {result.Bindings[index]}");
        }

        Console.WriteLine();
    }
}
