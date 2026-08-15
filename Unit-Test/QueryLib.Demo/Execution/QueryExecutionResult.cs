using QueryLib.Compilers;

namespace QueryLib.Demo.Execution;

public sealed record QueryExecutionResult(CompiledQuery CompiledQuery, QueryResult QueryResult);
