using QueryLib.Compilers;

namespace QueryLib.Demo;

public interface IQueryRunner
{
    Task RunAsync(CompiledQuery query, string connectionString);
}
