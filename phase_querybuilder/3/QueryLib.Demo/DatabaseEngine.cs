using QueryLib.Compilers;
using QueryLib.Interfaces;

namespace QueryLib.Demo;

public record DatabaseEngine(
    string Name,
    ICompiler Compiler,
    IQueryRunner Runner,
    IDbConnectionFactory ConnectionFactory
);
