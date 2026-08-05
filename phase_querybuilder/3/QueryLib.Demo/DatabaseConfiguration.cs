using QueryLib.Compilers;
using QueryLib.Interfaces;

namespace QueryLib.Demo;

public class DatabaseConfiguration
{
    public string Name { get; }

    public ICompiler Compiler { get; }

    public IQueryRunner Runner { get; }

    public string ConnectionString { get; }

    public DatabaseConfiguration(
        string name,
        ICompiler compiler,
        IQueryRunner runner,
        string connectionString)
    {
        Name = name;
        Compiler = compiler;
        Runner = runner;
        ConnectionString = connectionString;
    }
}

