using QueryLib.Compilers.Abstractions;
using QueryLib.Demo.Abstractions;

using System.Data.Common;


namespace QueryLib.Demo.Execution;

public sealed record QueryExecutionDependencies(
    ICompiler Compiler,
    IQueryRunner Runner,
    DbConnection Connection);