using System.Collections.Generic;
using QueryLib.Compilers;

namespace QueryLib.Compilers.Abstractions;

public interface ICompiler
{
    CompiledQuery Compile(Query query);
}


