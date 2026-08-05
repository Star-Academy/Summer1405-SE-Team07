using System.Collections.Generic;
using QueryLib.Compilers;

namespace QueryLib.Interfaces;

public interface ICompiler
{
    CompiledQuery Compile(Query query);
}


