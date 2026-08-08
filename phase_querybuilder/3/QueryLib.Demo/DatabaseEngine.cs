using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Interfaces;

namespace QueryLib.Demo;

public record DatabaseEngine(
    string Name,
    ICompiler Compiler,
    IQueryRunner Runner,
    IDbConnectionFactory ConnectionFactory
);
