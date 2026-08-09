using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo;

public record DatabaseTarget(
    string Name,
    ICompiler Compiler,
    IQueryRunner Runner,
    IDbConnectionFactory ConnectionFactory
);
