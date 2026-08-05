using QueryLib.Compilers;
using QueryLib.Interfaces;

namespace QueryLib.Demo;

public record DatabaseConfiguration(
    string Name,
    ICompiler Compiler,
    IQueryRunner Runner,
    IDbConnectionFactory ConnectionFactory
);
