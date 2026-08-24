using QueryLib.Demo;

namespace QueryLib.Compilers.Abstractions;

public interface ICompiler
{
    CompiledQuery Compile(Query query, DbProvider dbProvider);
}
