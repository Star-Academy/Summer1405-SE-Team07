using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;

namespace QueryLib.Demo.Execution;

public sealed record QueryExecutionDependencies(
    ICompiler Compiler,
    IQueryRunner Runner,
    IDbConnectionFactory ConnectionFactory);
